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
    public Light[] Lights;

    public Animator animator;
    public const string _Transform = "TransformController";

    public Coroutine flashStage;
    public Coroutine flashPress;
    public string[] ColorOrder = {"Blue", "Red", "Yellow", "Green"};
    public int[,] cubeNet = { { 100, 100, 1, 100, 100 }, { 100, 0, 2, 3, 100 }, { 100, 100, 4, 100, 100 }, { 100, 100, 5, 100, 100 } };
    public int[] flashes = { 0, 0, 0 };
    public int stage = 0;
    public int current;
    public int adjacent;
    public int Base10;
    public int[] position;
    public int Orientation;
    public int phase = 1;
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
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
        if (ModuleSolved)
        {
            return;
        }
        if (ModuleSolved)
        {
            return;
        }
        for (int i = 0; i < 6; i++)
        {
            if (Button == Buttons[i])
            {
                HandleButtonFlash(i);
                if (ButtonsToPress[0] == i)
                {
                    ButtonsToPress.RemoveAt(0);
                }
                else
                {
                    Debug.LogFormat("[My First Module #{0}] The button to press was {1}, but {2} was pressed", ModuleId, current, i);
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
        }
        if (ButtonsToPress.Count() == 0)
        {
            if (phase == 1)
            {
                animator.SetTrigger(_Transform);
                Displays[0].text = "";
                Displays[1].text = "";
                for (int i = 0; i < 3; i++)
                {
                    flashes[i] = Rnd.Range(0, 6);
                }
                flashStage = StartCoroutine(HandleStageFlash());
                phase++;
                Answer = new List<int>();
                Phase2();
            }
            else if (phase == 2 && stage < 2)
            {
                stage++;
                Phase2();
            }
            else if (phase == 2 && stage == 2)
            {
                stage++;
                Audio.PlaySoundAtTransform("mus_mett_applause", transform);
                GetComponent<KMBombModule>().HandlePass();
                ModuleSolved = true;
            }
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

    void Phase2()
    {
        adjacent = getAdjacent();
        current = flashes[stage];
        Orientation = getOrientation(adjacent);
        position = getPosition();
        switch (stage)
        {
            case 0:
                Move(0);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                Move(0);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                break;
            case 1:
                // Orientation = (Orientation + 1) % 4;
                Move(0);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                Move(3);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                Move(0);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                Move(3);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                break;
            case 2:
                Move(2);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                Move(3);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                Move(2);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                Move(2);
                Debug.LogFormat("[My First Module #{0}] Moved to {1}", ModuleId, current);
                break;
        }
        Answer.Add(current);
        foreach (int x in Answer){
            ButtonsToPress.Add(x);
        }
        Debug.LogFormat("[My First Module #{0}] The flashing color is {1} and the color to press for this stage is {2}, which makes total input {3}", ModuleId, flashes[stage], current, ButtonsToPress.ToString());
    }

    int getAdjacent()
    {
        if (stage == 0 && flashes[stage] != 1 && flashes[stage] != 4)
        {
            return 1;
        }
        else if (stage == 0){
            return 3;
        }
        if (stage == 1 && flashes[stage] != 0 && flashes[stage] != 3)
        {
            return 0;
        }
        else if (stage == 1)
        {
            return 4;
        }
        else if (stage == 2 && flashes[stage] != 5 && flashes[stage] != 2)
        {
            return 5;
        }
        else
        {
            return 4;
        }
    }


    int getOrientation(int adj) // Orientation is an integer that represents the number of 90 degree clockwise turns from straight up
    {
        switch (current)
        {
            case 0:
            switch (adj)
                {
                    case 1: return 0;
                    case 2: return 1;
                    case 4: return 2;
                    case 5: return 3;
                }
                break;
            case 1:
                switch (adj)
                {
                    case 5: return 0;
                    case 3: return 1;
                    case 2: return 2;
                    case 0: return 3;
                }
                break;
            case 2:
                switch (adj)
                {
                    case 1: return 0;
                    case 3: return 1;
                    case 4: return 2;
                    case 0: return 3;
                }
                break;
            case 3:
                switch (adj)
                {
                    case 1: return 0;
                    case 5: return 1;
                    case 4: return 2;
                    case 2: return 3;
                }
                break;
            case 4:
                switch (adj)
                {
                    case 2: return 0;
                    case 3: return 1;
                    case 5: return 2;
                    case 0: return 3;
                }
                break;
            case 5:
                switch (adj)
                {
                    case 4: return 0;
                    case 3: return 1;
                    case 1: return 2;
                    case 0: return 3;
                }
                break;
        }
        return 0;
    }

    int[] getPosition()
    {
        int[] temp = { 0, 0 };
        switch (current)
        {
            case 0:
                temp[0] = 1;
                temp[1] = 1;
                return temp;
            case 1:
                temp[0] = 0;
                temp[1] = 2;
                return temp;
            case 2:
                temp[0] = 1;
                temp[1] = 2;
                return temp;
            case 3:
                temp[0] = 1;
                temp[1] = 3;
                return temp;
            case 4:
                temp[0] = 2;
                temp[1] = 2;
                return temp;
            case 5:
                temp[0] = 3;
                temp[1] = 2;
                return temp;
        }
        return temp;
    }

    void Move(int direction)
    {
        direction = (direction + Orientation) % 4;
        switch (direction)
        {
            case 0:
                switch (current)
                {
                    case 0:
                        current = 1;
                        position[0] = 0;
                        position[1] = 2;
                        Orientation = (Orientation + 1) % 4;
                        break;
                    case 1:
                        current = 5;
                        position[0] = 3;
                        position[1] = 2;
                        break;
                    case 3:
                        current = 1;
                        position[0] = 0;
                        position[1] = 2;
                        Orientation = (Orientation + 3) % 4;
                        break;
                    default:
                        if (position[0] == 0)
                        {
                            position[0] = 3;
                        }
                        else { position[0] = position[0] - 1; }
                        current = cubeNet[position[0],position[1]];
                        break;

                }
                break;
            case 1:
                switch (current)
                {
                    case 1:
                        current = 3;
                        position[0] = 1;
                        position[1] = 3;
                        Orientation = (Orientation + 1) % 4;
                        break;
                    case 3:
                        current = 5;
                        position[0] = 3;
                        position[1] = 2;
                        Orientation = (Orientation + 2) % 4;
                        break;
                    case 4:
                        current = 3;
                        position[0] = 1;
                        position[1] = 3;
                        Orientation = (Orientation + 3) % 4;
                        break;
                    case 5:
                        current = 3;
                        position[0] = 1;
                        position[1] = 3;
                        Orientation = (Orientation + 2) % 4;
                        break;
                    default:
                        if (cubeNet[position[0], position[1] + 1] < 100)
                        {
                            position[1] = 1;
                        }
                        else { position[1] = position[1] + 1; }
                        current = cubeNet[position[0], position[1]];
                        break;

                }
                break;
            case 2:
                switch (current)
                {
                    case 0:
                        current = 4;
                        position[0] = 2;
                        position[1] = 2;
                        Orientation = (Orientation + 3) % 4;
                        break;
                    case 5:
                        current = 1;
                        position[0] = 0;
                        position[1] = 2;
                        break;
                    case 3:
                        current = 4;
                        position[0] = 2;
                        position[1] = 2;
                        Orientation = (Orientation + 1) % 4;
                        break;
                    default:
                        if (position[0] == 3)
                        {
                            position[0] = 0;
                        }
                        else { position[0] = position[0] + 1; }
                        current = cubeNet[position[0], position[1]];
                        break;

                }
                break;
            case 3:
                switch (current)
                {
                    case 1:
                        current = 0;
                        position[0] = 1;
                        position[1] = 1;
                        Orientation = (Orientation + 3) % 4;
                        break;
                    case 0:
                        current = 5;
                        position[0] = 3;
                        position[1] = 2;
                        Orientation = (Orientation + 2) % 4;
                        break;
                    case 4:
                        current = 0;
                        position[0] = 1;
                        position[1] = 1;
                        Orientation = (Orientation + 1) % 4;
                        break;
                    case 5:
                        current = 0;
                        position[0] = 1;
                        position[1] = 1;
                        Orientation = (Orientation + 2) % 4;
                        break;
                    default:
                        if (cubeNet[position[0], position[1] - 1] < 100)
                        {
                            position[1] = 3;
                        }
                        else { position[1] = position[1] - 1; }
                        current = cubeNet[position[0], position[1]];
                        break;

                }
                break;
        }

    }
    
    void moveSound()
    {
        Audio.PlaySoundAtTransform("move", transform);
    }

    void fillSound()
    {
        Audio.PlaySoundAtTransform("snd_bombsplosion", transform);
    }

    void endSound()
    {
        Audio.PlaySoundAtTransform("mus_sfx_hypergoner_laugh", transform);
    }



    IEnumerator HandleStageFlash()
    {
        while (stage < 3)
        {
            yield return new WaitForSeconds(3f);
            Lights[flashes[stage]].enabled = true;
            yield return new WaitForSeconds(0.8f);
            Lights[flashes[stage]].enabled = false;
        }

    }

    IEnumerator HandleButtonFlash(int pos)
    {
        Lights[pos].enabled = true;
        yield return new WaitForSeconds(0.8f);
        Lights[pos].enabled = false;
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
