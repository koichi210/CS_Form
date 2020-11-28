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

:: �t�@�C���쐬
echo Testdata01 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��01��.txt"
echo Testdata02 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��02��.txt"
echo Testdata03 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��03��.txt"
echo Testdata04 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��04��.txt"
echo Testdata05 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��05��.txt"
echo Testdata06 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��06��.txt"
echo Testdata07 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��07��.txt"
echo Testdata08 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��08��.txt"
echo Testdata09 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��09��.txt"
echo Testdata10 > %FILE_LIST_PATH%"�^�C�g��[�n��] ��10��.txt"

echo Sampledata01 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��01��.txt"
echo Sampledata02 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��02��.txt"
echo Sampledata03 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��03��.txt"
echo Sampledata04 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��04��.txt"
echo Sampledata05 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��05��.txt"
echo Sampledata06 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��06��.txt"
echo Sampledata07 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��07��.txt"
echo Sampledata08 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��08��.txt"
echo Sampledata09 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��09��.txt"
echo Sampledata10 > %FILE_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��10��.txt"

echo StackData11 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��11��.txt"
echo StackData12 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��12��.txt"
echo StackData13 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��13��.txt"
echo StackData14 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��14��.txt"
echo StackData15 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��15��.txt"
echo StackData16 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��16��.txt"
echo StackData17 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��17��.txt"
echo StackData18 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��18��.txt"
echo StackData19 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��19��.txt"
echo StackData20 > %STACK_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] ��20��.txt"

:: �f�B���N�g���쐬&�t�@�C���쐬
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��01��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��02��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��03��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��04��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��05��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��06��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��07��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��08��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��09��"
mkdir %DIR_LIST_PATH%"�^�C�g��[�n��] ��10��"

mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��01��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��02��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��03��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��04��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��05��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��06��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��07��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��08��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��09��"
mkdir %DIR_LIST_PATH%"�͂��߂���R�R�܂�[�v���o] ��10��"

::�ړI�̏ꏊ�쐬
mkdir %DEST_DIR_PATH%"�^�C�g��[�n��] �y�쐬���z_01-02"
mkdir %DEST_DIR_PATH%"�͂��߂���R�R�܂�[�v���o] �y�쐬���z_01-03"
