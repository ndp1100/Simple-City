﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BudgetManager : MonoBehaviour {


	public float money;
	public float revenue;
	private float countdown;

	[Header("Balance")]
	public float timePerPaycheck;
	public int paychecksPerConstruction;
	public float costToBuildIncreaseRate;
	public bool buildFloorsEveryConstruction;

	[Header("Setup")]

	public Text moneyText;
	public Text revenueText;
	public Text countdownText;
	public Slider countdownSlider;
	public Color positiveCashflowColor;
	public Color negativeCashflowColor;

	public static BudgetManager instance;

	private int numBuiltBuildings;
	private int paychecks;
	public bool paused;

	public bool Purchase( float price ) {
		if( money > price ) {
			money -= price;
			numBuiltBuildings++;
			UpdateCostToBuildForPlots();
			UpdateUI();
			return true;
		}
		PlayerUIManager.instance.NotEnoughtMoney();
		return false;
	}
	
// 	public void Pause() {
// 		paused = true;
// 	}
// 	
// 	public void Resume() {
// 		paused = false;
// 	}

	void Awake() {
		instance = this;
	}


	// Use this for initialization
	void Start () {
		paused = false;
		countdown = timePerPaycheck;
		CalculateRevenue();
		UpdateUI();
		numBuiltBuildings = 0;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCountdown();
	}

	void CalculateRevenue() {
		revenue = 0;
		foreach( Building thisBuilding in BuildingManager.instance.buildings ) {
			if( thisBuilding.isRunning )
				revenue += thisBuilding.GetComponent<BuildingRevenue>().revenue;
		}
	}

	void UpdateUI() {
		moneyText.text = "$" + money;

		if( revenue >= 0 ) {
			revenueText.text = "+$" + revenue;
			revenueText.color = positiveCashflowColor;
		}
		else {
			revenueText.text = "-$" + Mathf.Abs( revenue );
			revenueText.color = negativeCashflowColor;
		}
		countdownText.text = ( paychecks % paychecksPerConstruction + 1 ) + "/" + paychecksPerConstruction ;

	}

	void UpdateCountdown() {
		if( !paused ) {
			countdown -= Time.deltaTime;
		}
		if( countdown < 0 ) {
			countdown = timePerPaycheck;
			ApplyPaycheck();
		}
		countdownSlider.value = (timePerPaycheck - countdown)/timePerPaycheck;

	}

	void ApplyPaycheck() {
		paychecks++;
		if( buildFloorsEveryConstruction && paychecks%paychecksPerConstruction == 0) {
			BuildFloorOnAll();
		}
		CalculateRevenue();
		money += revenue;


		foreach( Building thisBuilding in BuildingManager.instance.buildings ) {
			if( thisBuilding.isRunning ) {
				thisBuilding.age++;
			}
		}

		UpdateUI();
	}

	void BuildFloorOnAll() {
		foreach( Building thisBuilding in BuildingManager.instance.buildings ) {
			thisBuilding.BuildFloor();
		}
		CalculateRevenue();
	}

	void UpdateCostToBuildForPlots() {
		foreach( Building thisBuilding in BuildingManager.instance.buildings ) {
			if( !thisBuilding.isBuilt ) {
// 				thisBuilding.GetComponent<BuildingRevenue>().UpdateCostToBuild( costToBuildIncreaseRate );
			}
		}
	}
}