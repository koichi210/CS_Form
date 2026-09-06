<#
.SYNOPSIS
    CS_Form配下の全テストプロジェクト(*.Tests.csproj)をまとめてビルド・実行するスクリプト。

.DESCRIPTION
    リファクタのたびに「各Testsプロジェクトを手でMSBuild→vstest.consoleで個別実行」を
    繰り返すのが非効率だったため、自動化した(重複撲滅への改善案#4)。

    1. リポジトリ配下の *.Tests.csproj を自動検出してすべてビルド
    2. ビルドできたテストDLLをまとめて1回のvstest.console実行にかける
    3. 合計・成功・失敗をまとめて表示し、失敗があれば非ゼロ終了コードを返す

    新しいTestsプロジェクトを追加しても、このスクリプト自体は変更不要
    (*.Tests.csprojの命名規則にさえ従っていれば自動的に拾われる)。

.PARAMETER Configuration
    ビルド構成。既定は Debug。

.EXAMPLE
    .\scripts\RunAllTests.ps1
    .\scripts\RunAllTests.ps1 -Configuration Release
#>
param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

# --- ツールのパスを決め打ちで探す(このPC環境の既知の場所) ---
$MSBuild = "D:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
$VsTest  = "D:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform\vstest.console.exe"

if (-not (Test-Path $MSBuild)) {
    Write-Error "MSBuild.exeが見つからないよ: $MSBuild `n(パスが変わってたら、このスクリプト冒頭の`$MSBuildを書き換えてね)"
    exit 1
}
if (-not (Test-Path $VsTest)) {
    Write-Error "vstest.console.exeが見つからないよ: $VsTest `n(パスが変わってたら、このスクリプト冒頭の`$VsTestを書き換えてね)"
    exit 1
}

# スクリプト自身の場所からリポジトリルート(scriptsの1つ上)を割り出す
$RepoRoot = Split-Path -Parent $PSScriptRoot

Write-Output "=== *.Tests.csproj を検出中... ($RepoRoot 配下) ==="
$testProjects = Get-ChildItem -Path $RepoRoot -Recurse -Filter "*.Tests.csproj" -File |
    Where-Object { $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\' }

if ($testProjects.Count -eq 0) {
    Write-Warning "*.Tests.csprojが1つも見つからなかったよ。"
    exit 1
}

Write-Output "$($testProjects.Count) 件のテストプロジェクトを検出:"
$testProjects | ForEach-Object { Write-Output "  - $($_.Name)" }
Write-Output ""

# --- ビルド ---
$builtDlls = @()
$buildFailures = @()

foreach ($proj in $testProjects) {
    Write-Output "=== Build: $($proj.Name) ==="
    # Platformは明示的に指定しない。各Testsプロジェクトは既定でx86を持っているが、
    # 外部から/p:Platform=x86を強制すると、依存先の本体プロジェクトがAnyCPU専用
    # (例: DialogChild)の場合にBaseOutputPath未設定エラーで壊れてしまうため。
    & $MSBuild $proj.FullName /p:Configuration=$Configuration /nologo /v:minimal
    if ($LASTEXITCODE -ne 0) {
        $buildFailures += $proj.Name
        Write-Output "  -> ビルド失敗"
        continue
    }

    $dllName = [System.IO.Path]::GetFileNameWithoutExtension($proj.Name) + ".dll"
    $dllPath = Join-Path $proj.DirectoryName "bin\$Configuration\$dllName"
    if (Test-Path $dllPath) {
        $builtDlls += $dllPath
    }
    else {
        Write-Warning "  -> ビルドは成功したはずだけどDLLが見つからない: $dllPath"
    }
}

Write-Output ""
if ($buildFailures.Count -gt 0) {
    Write-Warning "ビルド失敗したプロジェクト: $($buildFailures -join ', ')"
}

if ($builtDlls.Count -eq 0) {
    Write-Error "実行できるテストDLLが1つも無いよ。"
    exit 1
}

# --- テスト実行(まとめて1回) ---
Write-Output "=== $($builtDlls.Count) 件のテストDLLをまとめて実行 ==="
& $VsTest $builtDlls /Platform:x86 /Framework:.NETFramework,Version=v4.7.2

$testExitCode = $LASTEXITCODE

Write-Output ""
if ($buildFailures.Count -gt 0 -or $testExitCode -ne 0) {
    Write-Output "=== 結果: 失敗あり ==="
    exit 1
}
else {
    Write-Output "=== 結果: 全部成功 ==="
    exit 0
}
