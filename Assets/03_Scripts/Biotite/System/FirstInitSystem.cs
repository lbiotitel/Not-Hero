using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//
public class FirstInitSystem : MonoBehaviour
{
    [Header("Test")]
    [SerializeField] private List<string> testClassList;

    [Header("Properties")]
    [SerializeField] private SpriteRenderer[] characterSpawnRenderer;
    [SerializeField] private int spawnCount;
    private List<string> selectedClasses;

    private void Start()
    {
        selectedClasses = GetRandomClassList();
        ShowClassBodyRender();
    }

    //배틀매니저에 뽑은 클래스의 이름을 넘겨 세팅하게 합니다.
    public void SetHeroObjects()
    {
        if (testClassList.Count > 0)
        { 
            selectedClasses = testClassList;
        }
        Managers.Battle.SetHeroes(selectedClasses);
    }

   

    private void ShowClassBodyRender()
    {
        //뽑은 데이터들을 가져와서 renderer에 렌더링 합니다
        int rendererIndex = 0;
        foreach (var className in selectedClasses)
        {
            Debug.Log(className);
            HeroData heroData = Managers.Data.GetData<HeroData>(className);
            characterSpawnRenderer[rendererIndex++].sprite = heroData.Sprite;
        }
    }

    //랜덤한 클래스의 목록을 중복 없이 뽑습니다.
    private List<string> GetRandomClassList()
    { 
        HashSet<string> selectedClasses = new HashSet<string>();

        while (selectedClasses.Count != spawnCount)
        {
            int randomIndex = Random.Range(0, (int)EClassType.Last);

            string className = ((EClassType)randomIndex).ToString();

            selectedClasses.Add(className);
        }

        return selectedClasses.ToList<string>();
    }
}
