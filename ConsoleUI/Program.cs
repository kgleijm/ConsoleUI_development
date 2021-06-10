using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        // Option that exists in a menu but always returns default. Meaning no change in state is made.
        class GhostOption : Option
        {
            
            public override T Pick<T>()
            {
                return default;
            }

            public GhostOption(string optiontext) : base(optiontext)
            {
                
            }
        }
        

        // Option that takes a function with no input but returns the result of that function
        class FunctionalOption<returnType> : Option
        {
            private Func<returnType> myFunction; 
            
            public FunctionalOption(string text, Func<returnType> func) : base(text)
            {
                myFunction = func;
            }

            public override returnType Pick<returnType>()
            {
                var result = myFunction();
                return (returnType)(Object)result;
            }
        }

        // Option that, when picked,  executes an action(block of code) passed in the constructor
        class ActionOption : Option
        {

            private Action myAction;
            
            public ActionOption(string text, Action action) : base(text)
            {
                myAction = action;
            }

            public override T Pick<T>()
            {
                myAction();
                return default(T);
            }
        }
        
        
        // Abstract class that, when extended, allows a class to be picked as an option
        abstract class ObjectiveOption : Option
        {
            protected ObjectiveOption() : base("")
            {
            }

            public abstract override string AsString();
            
            public override T Pick<T>()
            {
                return (T)(object)this;
            }
        }
        
        
        // User Interface menu that lets the user pick an option, while being an option itself to allow for menu navigation
        class Menu : Option
        {
            private List<Option> Options;

            public Menu(string menuTitle) : base(menuTitle)
            {
                Options = new List<Option>();
            }

            public Menu(string menuTitle, List<Option> menuOptions) : base(menuTitle)
            {
                Options = menuOptions;
            }
            
            private void Print()
            {
                Console.Out.WriteLine(AsString());
                listOptions();
                Console.Out.WriteLine(" [x] Exit");
            }

            public void listOptions()
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    Console.Out.WriteLine(" [" + i + "] " + Options[i].AsString());
                }
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
                Pick<object>();
            }
        }
        
        class RandomNumberContainer : ObjectiveOption
        {

            private int myNumber;
            private static Random r = new Random();
            
            public RandomNumberContainer()
            {
                myNumber = r.Next(0,100);
            }
            
            public override string AsString()
            {
                return "Random number container containing: " + myNumber;
            }
        }

        
        void printOptionList(List<Option> list)
        {
            foreach (var option in list)
            {
                Console.Out.WriteLine(option.AsString());
            }
        }


        public static void Main(string[] args)
        {
            // Creating some test objects
            List<Option> randomNumberContainers = new List<Option>();
            for (int i = 0; i < 10; i++)
            {
                randomNumberContainers.Add(new RandomNumberContainer());
            }
            
            
            // Instantiating menu objects
            Menu mainMenu = new Menu("Main menu");
            Menu A = new Menu("Ghost options A");
            Menu B = new Menu("Ghost options B");
            
            // Instantiating test options
            GhostOption optionA = new GhostOption("Its all");
            GhostOption optionB = new GhostOption("The same");
            
            // Instantiating functional options
            ActionOption addRandomNumberContainer = new ActionOption("Add randomNumberContainer", () => randomNumberContainers.Add(new RandomNumberContainer()));
            FunctionalOption<RandomNumberContainer> chooseRandomNumberContainer = new FunctionalOption<RandomNumberContainer>("choose randomNumberContainer", () => (RandomNumberContainer)randomNumberContainers[new Random().Next(0, randomNumberContainers.Count -1)]);
            
            Menu chooseNumberContainerMenu = new Menu("Choose: ", randomNumberContainers);
            chooseNumberContainerMenu.addOption(chooseRandomNumberContainer);
            

            
            
            
            
            
            
            mainMenu.addOption(A);
            mainMenu.addOption(B);
            //main.addOption(actionPrintString);
            
            A.addOption(optionA);
            A.addOption(optionB);
            
            B.addOption(optionA);
            B.addOption(optionB);
            
            
            mainMenu.Start();




        }
    }
}