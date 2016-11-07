using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Syncano;
using Syncano.Data;
using Syncano.Enum;
using Newtonsoft.Json;

public class Test : MonoBehaviour {

	private SyncanoClient syncano;
	private Record record;

	IEnumerator Start() 
	{
		syncano = SyncanoClient.Instance.Init("8caf6f388a192347c326320ab21ce109715fca79", "examples");

		yield return StartCoroutine(CreateNewRecordObject());
	
		StartCoroutine(UpdateLeaderboard());
		StartCoroutine(SetScore());
	}

	private IEnumerator CreateNewRecordObject()
	{
		record = new Record("Test Nickname", 0); 
		record.OwnerPermissions = DataObjectPermissions.FULL;
		yield return syncano.Please().Save(record, OnRecordCreated, OnRecordNotCreated);
	}

	private IEnumerator SetScore()
	{
		while(true)
		{
			UpdateScore();
			yield return new WaitForSeconds(Random.Range(1, 5));
		}
	}

	private void UpdateScore()
	{
		record.Score+= 1;
		syncano.Please().Save(record, OnRecordUpdated, OnRecordUpdateFail);
	}

	private void OnRecordCreated(Response<Record> r)
	{
		record.Id = r.Data.Id;
		Debug.Log("success");
		// handle success
	}

	private void OnRecordNotCreated(Response<Record> r)
	{
		Debug.Log(r.syncanoError);
		// handle error
	}

	private void OnRecordUpdated(Response<Record> r)
	{
		// handle success
	}

	private void OnRecordUpdateFail(Response<Record> r)
	{
		// handle error
	}

	private IEnumerator UpdateLeaderboard()
	{
		while (true)
		{
			syncano.Please().RunScriptEndpointUrl("https://api.syncano.io/v1.1/instances/examples/endpoints/scripts/p/b853bc6446078776031aa88dc6d093e81e5b61a7/get_top_ten/", ScriptCallback);
			yield return new WaitForSeconds(3);
		}
	}

	private void ScriptCallback(ScriptEndpoint endpointCallback)
	{
		if(endpointCallback.IsSuccess)
		{
			List<Record> topTenRecords = JsonConvert.DeserializeObject<List<Record>>(endpointCallback.stdout, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			DisplayTopScore(topTenRecords);
		}

		else
		{
			if(endpointCallback.IsSyncanoError)
			{
				Debug.Log(endpointCallback.syncanoError);
			}

			else
			{
				Debug.Log(endpointCallback.webError);
			}
		}
	}

	private void DisplayTopScore(List<Record> list)
	{
		Debug.Log("*** TOP SCORE ***");

		for (int i = 0; i < list.Count; i++)
		{
			//Handle UI display
			Debug.Log("*** Nickname: " + list[i].Name + " Score: " + list[i].Score + " ***");
		}
	}
}