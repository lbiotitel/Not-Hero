using UnityEngine;

public class UnitData : ScriptableObject
{
    [SerializeField] string uName;
    [SerializeField] Sprite sprite;
    [SerializeField] RuntimeAnimatorController animControl;

    public Sprite Sprite => sprite;
    public string UName => uName;
    public RuntimeAnimatorController AnimControl => animControl;
}
