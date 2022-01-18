using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputOutput
{
    public static void Input(Card card) {
        foreach (Resource input in card.inputs)
        {
            Resources.Remove(input);
        }
    }
    public static void Output(Card card)
    {
        foreach (Resource output in card.outputs)
        {
            Resources.Add(output);
        }
    }
}
