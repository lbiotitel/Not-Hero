using System.Collections.Generic;
using UnityEngine;


class SpawnSystem : MonoBehaviour
{
    [SerializeField] GameObject pObj;
    [SerializeField] GameObject eObj;

    [SerializeField] Transform pSpwanTransform;
    [SerializeField] Transform eSpwanTransform;

    public List<Hero> SpawnPlayer(List<string> heroNames)
    {
        List<Hero> res = new List<Hero>();

        for(int i = 0; i < heroNames.Count; i++)
        {
            if(heroNames[i] == null || heroNames[i] == string.Empty)
            {
                Debug.Log("빈자리");
                continue;
            }

            HeroData heroData = Managers.Data.GetData<HeroData>(heroNames[i]);
            Vector3 spawnPos = SpawnPosition(pSpwanTransform, i, heroNames.Count, 1.8f);
            
            Hero hero = Instantiate(pObj, spawnPos, Quaternion.identity).GetComponent<Hero>();

            hero.transform.SetParent(pSpwanTransform);
            RandomStates(heroData.name, hero, heroData);
            res.Add(hero);

            foreach (var cardName in heroData.StartCardName)
            { 
                Managers.PlayerStat.AddNewCard(cardName, hero);
            }
            //playerCardLode
        }

        return res;
    }

    public List<Enemy> SpawnEnemy(List<string> enemyNames)
    {
        List<Enemy> res = new List<Enemy>();

        for(int i = 0; i < enemyNames.Count; i++)
        {
            if (enemyNames[i] == null || enemyNames[i] == string.Empty)
            {
                Debug.Log("빈자리");
                continue;
            }

            EnemyData enemyData = Managers.Data.GetData<EnemyData>(enemyNames[i]);
            Vector3 spawnPos = SpawnPosition(eSpwanTransform, i, enemyNames.Count, 1.8f);

            Enemy enemy = Instantiate(eObj, spawnPos, Quaternion.identity).GetComponent<Enemy>();
            
            RandomStates(enemy, enemyData);
            enemy.transform.SetParent(eSpwanTransform);
            res.Add(enemy);
        }

        return res;
    }


    public void RandomStates(string name, Hero player, HeroData data)
    {
        if (data == null)
        {
            Debug.Log("직업 없음");
            return;
        }
        
        float str = Mathf.FloorToInt(Random.Range(data.ClassData.MinSTR, data.ClassData.MaxSTR));
        float hp = Mathf.FloorToInt(Random.Range(data.ClassData.MinHP, data.ClassData.MaxHP));

        UnitStat stat = new UnitStat(str, 0, hp);

        player.SetData(data, stat);
    }

    public void RandomStates(Enemy enemy, EnemyData data)
    {
        if (data == null)
        {
            Debug.Log("없는 몬스터 입니다.");
            return;
        }

        float hp = Mathf.FloorToInt(Random.Range(data.EMinHp, data.EMaxHp));

        UnitStat stat = new UnitStat(data.EMinSTR, data.EBaseDEF, hp);

        enemy.SetData(data, stat);
    }

    private Vector3 SpawnPosition(Transform transform, int index, int count, float space)
    {
        float offset = (index - (count - 1) / 2f) * space;

        Vector3 pos = transform.position;
        pos.x += offset;

        return pos;
    }
}
