using System.Text;

namespace ConsoleR.AsciiArt.AsciiCharacters;

internal static class AsciiChars
{
    static string[] allCharsAscii = [
@"
    _    
   / \   
  / _ \  
 / ___ \ 
/_/   \_\",
@"
 ____  
| __ ) 
|  _ \ 
| |_) |
|____/ ",
@"
  ____ 
 / ___|
| |    
| |___ 
 \____|",
@"
 ____  
|  _ \ 
| | | |
| |_| |
|____/ ",
@"
 _____ 
| ____|
|  _|  
| |___ 
|_____|",
@"
 _____ 
|  ___|
| |_   
|  _|  
|_|    ",
@"
  ____ 
 / ___|
| |  _ 
| |_| |
 \____|",
@"
 _   _ 
| | | |
| |_| |
|  _  |
|_| |_|",
@"
 ___ 
|_ _|
 | | 
 | | 
|___|",
@"
     _ 
    | |
 _  | |
| |_| |
 \___/ ",
@"
 _  __
| |/ /
| ' / 
| . \ 
|_|\_\",
@"
 _     
| |    
| |    
| |___ 
|_____|",
@"
 __  __ 
|  \/  |
| |\/| |
| |  | |
|_|  |_|",
@"
 _   _ 
| \ | |
|  \| |
| |\  |
|_| \_|",
@"
  ___  
 / _ \ 
| | | |
| |_| |
 \___/ ",
@"
 ____  
|  _ \ 
| |_) |
|  __/ 
|_|    ",
@"
  ___  
 / _ \ 
| | | |
| |_| |
 \__\_\",
@"
 ____  
|  _ \ 
| |_) |
|  _ < 
|_| \_\",
@"
 ____  
/ ___| 
\___ \ 
 ___) |
|____/ ",
@"
 _____ 
|_   _|
  | |  
  | |  
  |_|  ",
@"
 _   _ 
| | | |
| | | |
| |_| |
 \___/ ",
@"
__     __
\ \   / /
 \ \ / / 
  \ V /  
   \_/   ",
@"
__        __
\ \      / /
 \ \ /\ / / 
  \ V  V /  
   \_/\_/   ",
@"
__  __
\ \/ /
 \  / 
 /  \ 
/_/\_\",
@"
__   __
\ \ / /
 \ V / 
  | |  
  |_|  ",
@"
 _____
|__  /
  / / 
 / /_ 
/____|",
@"__ _ 
 / _` |
| (_| |
 \__,_|",
@"_     
| |__  
| '_ \ 
| |_) |
|_.__/ "
];
   
    public static string GetAsciiArt(string text) {
        var resultList = new List<string>();
        foreach(var ch in text.ToUpper().ToCharArray()){
            var chCode = (int)ch;
            var index = chCode - 'A';
            if(index >=0 && index <= allCharsAscii.Length){
                resultList.Add(allCharsAscii[index]);
            }
        }

        return ToSingleLine(resultList);
    }

    private static string ToSingleLine(List<string> asciiChars)
    {
        var sb = new StringBuilder();
        var rowsCount = asciiChars[0].Split(Environment.NewLine).Length;
        for(var i = 0; i< rowsCount;i++){
            for (var j = 0; j< asciiChars.Count;j++) {
                sb.Append(asciiChars[j].Split(Environment.NewLine)[i]);
            }
            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
}
