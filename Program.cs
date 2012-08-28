using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace zivs3
{
    class Program
    {
        static void Main(string[] args)
        {
            string div = "01";
            string inputMessage="";
            FileStream stream = File.Open("./mess.txt", FileMode.Open, FileAccess.Read);
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream,Encoding.Unicode);
                while (true)
                {
                    string readedLine = reader.ReadLine();
                    if (readedLine == null)
                    {
                        break;
                    }

                    inputMessage += readedLine;
                }
                reader.Close();
                stream.Close();
            }
            
            string codedInputMessage = "";
            string decodedInputMessage = "";

            List<Letter> letters = new List<Letter>();
            
            // подсчет частоты встречаемости каждой буквы
            foreach (char inputMessageLetter in inputMessage)
            {
                bool flagContains = false;
                foreach (Letter letter in letters)
                {

                    if (letter.symbol == inputMessageLetter)
                    {
                        letter.frequency++;
                        flagContains = true;
                        break;
                    }
                }
                if (!flagContains)
                {
                    letters.Add(new Letter {frequency = 1, symbol = inputMessageLetter});
                }
            }

            // сортировка букв по частоте встречаемости
            letters.Sort(CompareByFrequency);

            // выбор кодировок 
            int[] val = new int[1];
            int codedLettersCount = 0;
            int bitsCount = 1;
            while (codedLettersCount < letters.Count())
            {
                for (int i = 0; i < Math.Pow(2, bitsCount); i++)
                {
                    val[0] = i;
                    BitArray code = new BitArray(val);
                    string codeString = "";

                    for (int j = bitsCount-1; j >= 0 ; j--)
                    {
                        codeString += code[j] ? "1" : "0";
                    }

                    if (codeString.IndexOf(div) == -1)
                    {
                        letters[codedLettersCount].code = codeString;
                        codedLettersCount++;
                        if (codedLettersCount == letters.Count)
                        {
                            break;
                        }
                    }
                }
                bitsCount++;
            }

            // кодирование сообщения
            foreach (char inputMessageLetter in inputMessage)
            {
                foreach (Letter letter in letters)
                {
                    if(inputMessageLetter == letter.symbol)
                    {
                        codedInputMessage += letter.code;
                        codedInputMessage += div;
                    }
                }
            }

            // подсчет длин
            int inputMessageLength = inputMessage.Length * 8;
            int codedInputMessageLength = codedInputMessage.Length;

            Console.WriteLine("Разделитель: "+div);
            Console.WriteLine("Символы, частота их встречаемости, новая кодировка: ");
            for (int i = 0; i < letters.Count; i++)
            {
                Console.Write(letters[i].symbol.ToString() + "  " + letters[i].frequency.ToString() + "  " + letters[i].code);
                Console.WriteLine("");
            }
            Console.WriteLine("Размер в битах исходного сообщения: "+inputMessageLength.ToString());
            Console.WriteLine("Размер в битах сжатого сообщения: " + codedInputMessageLength.ToString());
            Console.WriteLine("Процент сжатия: " + ((int)((double)codedInputMessageLength/(double)inputMessageLength*100)).ToString()+"%");
            // декодирование

            string codeDecodingLetter = "";
            for (int i = 0; i < codedInputMessage.Length - 1; i++)
            {                
                if (codedInputMessage[i] == '0' && codedInputMessage[i+1] == '1')
                {
                    i++;
                    foreach (Letter letter in letters)
                    {
                        if(letter.code == codeDecodingLetter)
                        {
                            decodedInputMessage += letter.symbol;
                        }
                    }
                    codeDecodingLetter = "";
                    continue;
                }
                else
                {
                    codeDecodingLetter += codedInputMessage[i];
                }
            }
            stream = File.Open("./codedInputMessage.txt", FileMode.Create, FileAccess.Write);
            if (stream != null)
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(codedInputMessage);
                writer.Close();
                stream.Close();
            }
            stream = File.Open("./OutMessage.txt", FileMode.Create, FileAccess.Write);
            if (stream != null)
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(decodedInputMessage);
                writer.Close();
                stream.Close();
            }
            Console.ReadLine();
        }

        private static int CompareByFrequency(Letter letter1, Letter letter2)
        {
            if(letter1.frequency > letter2.frequency)
            {
                return -1;
            }
            if (letter1.frequency < letter2.frequency)
            {
                return 1;
            }
            if (letter1.frequency == letter2.frequency)
            {
                return 0;
            }
            return 0;
        }
    }

    class Letter
    {
        public char symbol;
        public int frequency;
        public string code;
    }
}
