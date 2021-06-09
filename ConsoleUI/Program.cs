using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleUI
{
    internal class Program
    {
        
        abstract class Option
        {
            
            private string optionText;

            public virtual string AsString()
            {
                return optionText;
            }

            public abstract T Pick<T>();
            
            public Option(string text)
            {
                optionText = text;
            }

        }

        class GhostOption : Option
        {
            
            
            public override T Pick<T>()
            {
                return default(T);
            }

            public GhostOption(string optiontext) : base(optiontext)
            {
                
            }
        }

        class FunctionalOption<T> : Option
        {
            private Func<T> myFunction; 
            
            public FunctionalOption(string text, Func<T> func) : base(text)
            {
                myFunction = func;
            }

            public override T1 Pick<T1>()
            {
                var result = myFunction();
                return (T1)(Object)result;
            }
        }
            
        class Menu : Option
        {
            private List<Option> Options;

            public Menu(string menuTitle) : base(menuTitle)
            {
                Options = new List<Option>();
            }
            
            private void Print()
            {
                Console.Out.WriteLine(AsString());
                for (int i = 0; i < Options.Count; i++)
                {
                    Console.Out.WriteLine(" [" + i + "] " + Options[i].AsString());
                }
                Console.Out.WriteLine(" [x] Exit");
            }

            
            public override T Pick<T>()
            {
                

                T result = default(T);
                while (result == null || result.Equals(default(T)))
                {
                    Console.Clear();
                    Print();
                    
                    string inp = Console.ReadLine();
                    if (inp == "x")
                    {
                        return default(T);
                    }
                    
                    try
                    {
                        result = Options[Int32.Parse(inp)].Pick<T>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Not a valid option, pick an option from the list");
                    }
                }

                return result;
            }

            public void addOption(Option o)
            {
                Options.Add(o);
            }

            public void Start()
            {
                Pick<Object>();
            }
        }
        
        

        




        public static void Main(string[] args)
        {
            Menu main = new Menu("Main menu");
            Menu A = new Menu("Menu A");
            Menu B = new Menu("Menu B");
            
            GhostOption optionA = new GhostOption("Its all");
            GhostOption optionB = new GhostOption("The same");
            
            
            FunctionalOption<String> funcReturnStringA = new FunctionalOption<string>("Return \"A\"", () => "I'm printing A, returning after 3 sec");
            FunctionalOption<String> funcReturnStringB = new FunctionalOption<string>("Return \"B\"", () => "I'm printing B, returning after 3 sec");
            FunctionalOption<Object> funcPrintString = new FunctionalOption<object>("choose and print String Function",
                () =>
                {
                    Menu choice = new Menu("Pick one");
                    choice.addOption(funcReturnStringA);
                    choice.addOption(funcReturnStringB);
                    String result = choice.Pick<string>();

                    if (result.Equals(default(string)))
                    {
                        return null;
                    }
                    
                    Console.Out.WriteLine(result);
                    Thread.Sleep(3000);
                    return null;
                });
            
            
            main.addOption(A);
            main.addOption(B);
            main.addOption(funcPrintString);
            
            A.addOption(optionA);
            A.addOption(optionB);
            
            B.addOption(optionA);
            B.addOption(optionB);
            
            main.Start();




        }
    }
}