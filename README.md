# Excel2Json

[![LICENSE](https://img.shields.io/badge/license-NPL%20(The%20996%20Prohibited%20License)-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>

this tools used to export excel data to json.

### How To Use?

- param1: xlsx file path
- param2: output json file path , 

if param2 is null , the default path is the path of the xlsx file, followed by ".json"

Shell:

> xls2json "xxx/xxx/xxx/aaa.xlsx" "xxx/xxx/xxx/aaa.json"

PowerShell:

> .\xls2json.exe "C:\roleInfo.xlsx" "C:\roleInfo.json"



