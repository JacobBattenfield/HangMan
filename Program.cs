using System.Collections;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

class Program{
    static async Task Main(){ //The First Thing That Is Called
        await StartGame();
    }
    public static void GetWordBlanks(string word, char guess){ //The Fourth Thing That Is Called
        if(guess=='*'){
            Console.WriteLine("Starting Game!");
            Thread.Sleep(2000);
        }
        int len = word.Length;
        int attempts = (int)Math.Round((0.5*len)+6);
        char[] wordProgress = new char[len];
        for(int i = 0;i<len;i++){
            wordProgress[i]='-';
        }
        string Upperword = word.ToUpper();
        Guess(Upperword,wordProgress,attempts,"");
    }
    public static async Task StartGame(){ //The Second Thing That Is Called
        string word = await FetchRandomWord();
        if(!string.IsNullOrEmpty(word)){
          GetWordBlanks(word, '*');  
        }
    }
    static async Task<string>FetchRandomWord(){ //The Third Thing That Is Called
        try{
            using HttpClient client = new();
            string apiURL = "https://random-word-api.herokuapp.com/word";
            HttpResponseMessage response = await client.GetAsync(apiURL);

            if(response.IsSuccessStatusCode){
                string json = await response.Content.ReadAsStringAsync();
                string[] words = JsonSerializer.Deserialize<string[]>(json)!;
                return words?[0]??string.Empty;
            }
        }
        catch(Exception ex){
            Console.WriteLine($"Error: Unable To Fetch Word {ex}");
        }
        return string.Empty;
    }
   static void Guess(string word, char[] wordProgress, int attempts, string used){ //The 5th, 10th, 15th, 20th.... Thing That Is Called
    attempts--;
    if (attempts == 0){
        Console.WriteLine("Unfortunately You Have Ran Out Of Attempts");
        Console.WriteLine();
        Console.WriteLine("-----GAME OVER------");
        Console.WriteLine("The Word Was " + word);
        return;
    }
    if (IsCompleted(wordProgress)){
        Console.WriteLine();
        Console.WriteLine("Congratulations! You Have Beat My Silly Hangman Game!");
        Console.WriteLine("The Word Was " + word);
        return;
    }
    Console.WriteLine();
    Console.WriteLine("Type In A Letter Guess, You Have " + attempts + " Attempts Left");
    Console.WriteLine(string.Join(" ", wordProgress));
    Console.Write("Your Letter Guess: ");

    char letter = GetValidLetter();

    if (FillInWordProgress(letter, word, wordProgress, attempts)){
        attempts++; 
    }
    string remainingLetters = StoreGuesses(letter, used);
    Console.WriteLine(remainingLetters);
    Guess(word, wordProgress, attempts, used + letter);
}
    static char GetValidLetter(){//The Seventh Thing THat Is Called
        while(true){
            string input = Console.ReadLine()!.Trim().ToUpper();
            if(input.Length==1&&char.IsLetter(input[0])){
                return input[0];
            }else{
                Console.WriteLine("Error: Invalid Input. Please Enter A Single Letter");
                Console.WriteLine();
            }
        }
    }

    static bool FillInWordProgress(char guess, string word, char[] wordProgress,int attempts){ //The Eighth Thing That Is Called
        bool thing = false;
        for(int i = 0;i<word.Length;i++){
            if(word[i]==guess){
                wordProgress[i]=guess;
                thing =true;
            }
        }
        if(!thing){
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("No Letters In The Word Match Your Guess");
            Console.WriteLine("---------------------------------------");
        }else if(thing){Console.WriteLine("---------------------------------------");
            Console.WriteLine("You Guessed A Letter Correct, Congrats!");
            Console.WriteLine("---------------------------------------");}
        return thing;
    }
    static string StoreGuesses(char guess, string used){ //The Ninth Thing That Is Called
    if (!used.Contains(guess)){
        used += guess;
    }
    string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string result = "";
    
        foreach (char letter in letters)
    {
        result += used.Contains(letter) ? '*' : letter;
    }
    
    return result;
}
    static bool IsCompleted(char[]wordProgress){ //The Sixth Thing That Is Called
        for(int i = 0;i<wordProgress.Length;i++){
            if(wordProgress[i]=='-'){
                return false;
            }
        }
        return true;
    }
}