using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class Template : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

    public KMSelectable[] Buttons;
    public TextMesh[] Displays;

    public Animator animator;
    public const string _Transform = "TransformController";

    public string[] ColorOrder = {"Blue", "Red", "Yellow", "Green"};
    public int Base10;
    public int FinalBase;
    List<int> Answer = new List<int>();
    List<int> ButtonsToPress = new List<int>();

    static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      foreach (KMSelectable Button in Buttons) {
          Button.OnInteract += delegate () { ButtonPress(Button); return false; };
      }
      

   }

    void ButtonPress (KMSelectable Button)
    {
        Button.AddInteractionPunch();
        if (ModuleSolved)
        {
            return;
        }

        if (ModuleSolved)
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            if (Button == Buttons[i])
            {
                if (ButtonsToPress[0] == i)
                {
                    ButtonsToPress.RemoveAt(0);
                }
                else
                {
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
        }
        if (ButtonsToPress.Count() == 0)
        {
            animator.SetTrigger(_Transform);
            Displays[0].text = "";
            Displays[1].text = "";
            GetComponent<KMBombModule>().HandlePass();
            ModuleSolved = true;
        }
    }

   void Start () {
        animator = GetComponent<Animator>();
        Base10 = Rnd.Range(40, 1000);
        Displays[0].text = Base10.ToString();
        FinalBase = Rnd.Range(4, 10);
        Displays[1].text = FinalBase.ToString();

        Debug.LogFormat("[My First Module #{0}] The displayed initial number is {1}, and the displayed base is {2}.", ModuleId, Displays[0].text, Displays[1].text);

        Calculation();
   }

    void Calculation ()
    {

        while (Base10 > 0)
        {
            Answer.Add(Base10 % FinalBase);
            Base10 = Base10 / FinalBase;
        }
        Answer.Reverse();
        int Total = 0;
        foreach (int Value in Answer)
        {
            Total = 10 * Total + Value;
        }
        Debug.LogFormat("[My First Module #{0}] The number after base conversion is {1}.", ModuleId, Total);

        if (FinalBase % 4 == 0) { Debug.LogFormat("[My First Module #{0}] The color order with a number of {1} is Blue, Red, Yellow, Green. ", ModuleId, FinalBase); }
        if (FinalBase % 4 == 1)
        {
            Debug.LogFormat("[My First Module #{0}] The color order with a number of {1} is Red, Yellow, Green, Blue. ", ModuleId, FinalBase);
            for (int i = 0; i < Answer.Count; i++)
            {
                Answer[i] += 3;
            }
        }
        if (FinalBase % 4 == 2)
        {
            Debug.LogFormat("[My First Module #{0}] The color order with a number of {1} is Yellow, Green, Blue, Red. ", ModuleId, FinalBase);
            for (int i = 0; i < Answer.Count; i++)
            {
                Answer[i] += 2;
            }
        }
        if (FinalBase % 4 == 3)
        {
            Debug.LogFormat("[My First Module #{0}] The color order with a number of {1} is Green, Blue, Red, Yellow. ", ModuleId, FinalBase);
            for (int i = 0; i < Answer.Count; i++)
            {
                Answer[i] += 1;
            }
        }
        foreach (int value in Answer)
        {
            if (value % 4 == 0)
            {
                ButtonsToPress.Add(0);
                
            }
            if (value % 4 == 1)
            {
                ButtonsToPress.Add(1);
            }
            if (value % 4 == 2)
            {
                ButtonsToPress.Add(2);
            }
            if (value % 4 == 3)
            {
                ButtonsToPress.Add(3);
            }
        }
        string Colors = "";
        foreach (int Value in ButtonsToPress)
        {
            if (Value == 0) { Colors += " Blue"; }
            if (Value == 1) { Colors += " Red"; }
            if (Value == 2) { Colors += " Yellow"; }
            if (Value == 3) { Colors += " Green"; }
        }
        Debug.LogFormat("[My First Module #{0}] The buttons to press are{1}", ModuleId, Colors);
    }

   void Update () {

   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} R/G/B/Y or Red, Green, Blue, Yellow. Chain commands using spaces";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
        Command = Command.Trim().ToUpper();
      yield return null;
        string[] Commands = Command.Split(' ');
        for (int i = 0; i < Commands.Length; i++)
        {
            if ((!"RBGY".Contains(Commands[i][0]) && Commands[i].Length < 2) && !(Commands[i] == "RED" || Commands[i] == "Green" || Commands[i] == "Blue" || Commands[i] == "Yellow"))
            {
                yield return "sentochaterror invalid command";
                yield break;
            }
        }
        for (int i = 0; i < Commands.Length; i++)
        {
            if (Commands[i] == "R" || Commands[i] == "RED") { Buttons[1].OnInteract(); }
            if (Commands[i] == "G" || Commands[i] == "GREEN") { Buttons[3].OnInteract(); }
            if (Commands[i] == "B" || Commands[i] == "BLUE") { Buttons[0].OnInteract(); }
            if (Commands[i] == "Y" || Commands[i] == "YELLOW") { Buttons[2].OnInteract(); }
            yield return new WaitForSeconds(.1f);
        }
    }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
