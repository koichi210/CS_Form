set ROOT_DIR="Work"

set FILE_LIST_PATH=%ROOT_DIR%\"FileList\"
set DIR_LIST_PATH=%ROOT_DIR%\"DirList\"
set DEST_DIR_PATH=%ROOT_DIR%\"Dest\"
set STACK_DIR_PATH=%ROOT_DIR%\"Stack\"

mkdir %ROOT_DIR%
mkdir %FILE_LIST_PATH%
mkdir %DIR_LIST_PATH%
mkdir %DEST_DIR_PATH%
mkdir %STACK_DIR_PATH%

:: ファイル作成
echo Testdata01 > %FILE_LIST_PATH%"タイトル[地域] 第01作.txt"
echo Testdata02 > %FILE_LIST_PATH%"タイトル[地域] 第02作.txt"
echo Testdata03 > %FILE_LIST_PATH%"タイトル[地域] 第03作.txt"
echo Testdata04 > %FILE_LIST_PATH%"タイトル[地域] 第04作.txt"
echo Testdata05 > %FILE_LIST_PATH%"タイトル[地域] 第05作.txt"
echo Testdata06 > %FILE_LIST_PATH%"タイトル[地域] 第06作.txt"
echo Testdata07 > %FILE_LIST_PATH%"タイトル[地域] 第07作.txt"
echo Testdata08 > %FILE_LIST_PATH%"タイトル[地域] 第08作.txt"
echo Testdata09 > %FILE_LIST_PATH%"タイトル[地域] 第09作.txt"
echo Testdata10 > %FILE_LIST_PATH%"タイトル[地域] 第10作.txt"

echo Sampledata01 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第01作.txt"
echo Sampledata02 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第02作.txt"
echo Sampledata03 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第03作.txt"
echo Sampledata04 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第04作.txt"
echo Sampledata05 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第05作.txt"
echo Sampledata06 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第06作.txt"
echo Sampledata07 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第07作.txt"
echo Sampledata08 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第08作.txt"
echo Sampledata09 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第09作.txt"
echo Sampledata10 > %FILE_LIST_PATH%"はじめからココまで[思い出] 第10作.txt"

echo StackData11 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第11作.txt"
echo StackData12 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第12作.txt"
echo StackData13 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第13作.txt"
echo StackData14 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第14作.txt"
echo StackData15 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第15作.txt"
echo StackData16 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第16作.txt"
echo StackData17 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第17作.txt"
echo StackData18 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第18作.txt"
echo StackData19 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第19作.txt"
echo StackData20 > %STACK_DIR_PATH%"はじめからココまで[思い出] 第20作.txt"

:: ディレクトリ作成&ファイル作成
mkdir %DIR_LIST_PATH%"タイトル[地域] 第01作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第02作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第03作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第04作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第05作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第06作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第07作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第08作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第09作"
mkdir %DIR_LIST_PATH%"タイトル[地域] 第10作"

mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第01作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第02作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第03作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第04作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第05作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第06作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第07作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第08作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第09作"
mkdir %DIR_LIST_PATH%"はじめからココまで[思い出] 第10作"

::目的の場所作成
mkdir %DEST_DIR_PATH%"タイトル[地域] 【作成中】_01-02"
mkdir %DEST_DIR_PATH%"はじめからココまで[思い出] 【作成中】_01-03"
