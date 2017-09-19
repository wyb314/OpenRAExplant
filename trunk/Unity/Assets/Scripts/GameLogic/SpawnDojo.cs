using UnityEngine;
    [System.Serializable]
    public class Round
    {
        [System.Serializable]
        public class RoundInfo
        {
            [System.Serializable]
            public class SpawnInfo
            {
                public E_EnemyType EnemyType;
                public SpawnPointEnemy[] SpawnPoint;
                public float SpawnDelay = 0;
                public bool RotateToPlayer = true;
            }

            public SpawnInfo[] Spawns;
            public float SpawnDelay = 0;
            public int MinEnemiesFomLastRound = 0;
        }

        public RoundInfo[] SpawnRounds;
    }

    public Round[] Rounds;

    public SpawnPointEnemy[] SpawnPoints = null;

    private GameObject GameObject;

    private GameZone MyGameZone;

	public bool IsActive() { return EnemiesAlive.Count > 0; }
    public Agent GetEnemy(int index) { return EnemiesAlive[index]; }
    public int GetEnemyCount() { return EnemiesAlive.Count; }

    public void Enable()
    {
    }
            for (int i = 0; i < SpawnPoints.Length; i++)
                Gizmos.DrawLine(GameObject.transform.position , SpawnPoints[i].transform.position);
        }

	// Update is called once per frame
        for (int i = EnemiesAlive.Count - 1; i >= 0; i--)
        {
            if (EnemiesAlive[i].IsAlive == true)
                continue;

            EnemiesAlive.RemoveAt(i);
        }
		}


            StartCoroutine(SpawnEnemy(SpawnPoints[i]));

        yield return new WaitForSeconds(4.0f);

    IEnumerator SpawnEnemy(SpawnPointEnemy spawnpoint)
    {
        CombatEffectsManager.Instance.PlaySpawnEffect(spawnpoint.Transform.position, spawnpoint.Transform.forward);

        yield return new WaitForSeconds(0.1f);

        GameObject enemy = Mission.Instance.GetHuman(spawnpoint.EnemyType, spawnpoint.Transform);


        Agent agent = enemy.GetComponent("Agent") as Agent;
        agent.PrepareForStart();
     
        if (spawnpoint.SpawnAnimation != null)
            agent.PlayAnim(spawnpoint.SpawnAnimation.name);

        MyGameZone.AddEnemy(agent);
        EnemiesAlive.Add(agent);

    IEnumerator SpawnEnemiesInRounds(Round.RoundInfo[] SpawnRounds)
    {
        State = E_State.E_SPAWNING_ENEMIES;

        for (int i = 0; i < SpawnRounds.Length; i++)
        {
            Round.RoundInfo round = SpawnRounds[i];

            float delay = round.SpawnDelay;

            while (delay > 0)
            {
                if (EnemiesAlive.Count == 0 || EnemiesAlive.Count <= round.MinEnemiesFomLastRound)
                    break;

                yield return new WaitForSeconds(0.5f);
                delay -= 0.5f;
            }

            for(int ii = 0; ii < round.Spawns.Length; ii++)
            {
                Round.RoundInfo.SpawnInfo spawnInfo = round.Spawns[ii];

                yield return new WaitForSeconds(spawnInfo.SpawnDelay);

                SpawnPointEnemy spawnpoint = GetAvailableSpawnPoint(spawnInfo.SpawnPoint == null || spawnInfo.SpawnPoint.Length == 0 ? SpawnPoints : spawnInfo.SpawnPoint);

                if (spawnInfo.RotateToPlayer)
                {
                    Vector3 dir = Player.Instance.Agent.Position - spawnpoint.Transform.position;
                    dir.Normalize();
                    spawnpoint.Transform.forward = dir;
                }

                GameObject enemy = Mission.Instance.GetHuman(spawnInfo.EnemyType, spawnpoint.Transform);

                while (enemy == null)
                {
                    yield return new WaitForSeconds(0.2f);
                    enemy = Mission.Instance.GetHuman(spawnInfo.EnemyType, spawnpoint.Transform);
                }

                CombatEffectsManager.Instance.PlaySpawnEffect(spawnpoint.Transform.position, spawnpoint.Transform.forward);

                Agent agent = enemy.GetComponent("Agent") as Agent;
                agent.PrepareForStart();
                
                MyGameZone.AddEnemy(agent);
                EnemiesAlive.Add(agent);
                yield return new WaitForSeconds(0.1f);
            }
        }

        State = E_State.E_IN_PROGRESS;
    }

    SpawnPointEnemy GetAvailableSpawnPoint(SpawnPointEnemy[] spawnPoints)
    {
        Vector3 pos = Player.Instance.Agent.Position;

        float bestValue = 0;
        int bestSpawn = -1; 

        for(int i = 0; i < spawnPoints.Length;i++)
        {
            if (MyGameZone.IsEnemyInRange(spawnPoints[i].transform.position, 2))
            {
            //    Debug.Log(i + " Spawnpoint " + spawnPoints[i].name + " is near to enemy");
                continue;
            }

            float value = 0;
            float dist = Mathf.Min(14, (spawnPoints[i].Transform.position - pos).magnitude);
            value = Mathfx.Hermite(0, 7, dist/7);

           // Debug.Log(i + " Spawnpoint " + spawnPoints[i].name + " dist " + dist + " Value " + value);
            if (value <= bestValue)
                continue;

            bestValue = value;
            bestSpawn = i;
        }

        //Debug.Log("Best spaqwn point is " + bestSpawn);

        if( bestSpawn == -1)
            return spawnPoints[Random.Range(0, spawnPoints.Length)];

        return spawnPoints[bestSpawn];
    }