using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob
{
    public class SubState
    {
        public int Id;
        public string Name;
        public Action Enter;
        public Action Update;
        public Action Exit;
    }
}
