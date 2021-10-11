using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob
{
    public class SubState
    {
        public int Id;
        public Action Enter;
        public Action Update;
        public Action Exit;
    }
}
