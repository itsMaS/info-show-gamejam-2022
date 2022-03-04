using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : Mutable
{
    public class Gene
    {
        public string name;
        public string description;
    }

    public class Genome
    {
        List<Gene> Genes = new List<Gene>();
    }


}
