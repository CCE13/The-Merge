using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDissolve
{
    public void Dissolve();
    public IEnumerator TransitionOut();
    public IEnumerator TransitionIn();
}
