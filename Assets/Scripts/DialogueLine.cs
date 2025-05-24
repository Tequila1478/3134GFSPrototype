using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
        [TextArea]
        public string text;              // The dialogue line
        public string heading;           // Speaker name (e.g., "Ghost")
        public Sprite characterSprite;   // Reference to character image
        public bool spriteOnRight;       // Layout direction
    
}
