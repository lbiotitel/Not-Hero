using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Unit/Class")]
public class CHRClassData : ScriptableObject
{
    [SerializeField] private EClassType CHRclass;

	[SerializeField] private float minHP;
    [SerializeField] private float maxHP;

    [SerializeField] private float minSTR;
    [SerializeField] private float maxSTR;

    public EClassType CHRClass => CHRclass;
    public float MinHP => minHP;
    public float MaxHP => maxHP;
    public float MinSTR => minSTR;
    public float MaxSTR => maxSTR;
}