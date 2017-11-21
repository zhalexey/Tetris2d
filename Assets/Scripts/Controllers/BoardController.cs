using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardController : MonoBehaviour
{

	public const int BOARD_WIDTH = 11;
	public const int BOARD_HEIGHT = 15;
	public const int BOARD_HALF_WIDTH = BOARD_WIDTH / 2;
	public const int BOARD_HALF_HEIGHT = BOARD_HEIGHT / 2;
	public const float BRICK_SIZE = 0.3f;
	public const float BRICK_HALF_SIZE = BRICK_SIZE / 2 - 0.02f;

	public const string PLAYER_TAG = "Player";
	public const string UNTAGGED = "Untagged";
	public const string COIN_TAG = "Coin";
	public const string HIT_TAG = "Hit";

	private const int ENERGY_ZONE_HEIGHT = BOARD_HALF_HEIGHT;
	private const int CALM_ZONE_HEIGHT = BOARD_HALF_HEIGHT + 3;

	private const float MIN_DOWN_DISTANCE = 0.01f;
	private const float MIN_DELTA_H = 0.028f;
	private const float MAX_DELTA_H = 0.032f;

	public List<GameObject> levelFigures;
	public List<GameObject> figures;
	public List<Sprite> brickTypes;
	public GameObject dropCoinSfx;
	public GameObject treasureBox;

	private Vector2 initPosition;
	private bool calmZoneState;
	private int coinsCount;


	void Awake ()
	{
		initPosition = getPos (new Vector2 (BoardController.BOARD_WIDTH / 2 - 1, 0));
	}

	void Start ()
	{
		coinsCount = 0;
		calmZoneState = true;
		ScriptManager.SoundController.PlayCalmMusic ();

		// level testing
		foreach (GameObject figure in levelFigures) {
			Instantiate (figure, figure.transform.position, Quaternion.identity);
		}
	}


	void Update ()
	{
		CheckBoardStatus ();
		ApplyMusic ();
	}


	private void ApplyMusic ()
	{

		MusicZoneHelper musicZone = CheckMusicZone ();

		if (calmZoneState && musicZone.isEnergyZoneReached ()) {
			calmZoneState = false;
			ScriptManager.SoundController.PlayEnergyMusic ();
		} else if (!calmZoneState && musicZone.isCalmZoneNotReached ()) {
			calmZoneState = true;
			ScriptManager.SoundController.PlayCalmMusic ();
		}

	}

	//--------------------------------------------- Next figure ------------------------------------------



	private GameObject GetNextFigure ()
	{
		GameObject figure = figures [UnityEngine.Random.Range (0, figures.Count)];
		return figure;
	}


	void RandomizeFigureTextures (GameObject figure)
	{
		Sprite sprite = brickTypes [UnityEngine.Random.Range (1, 7)];
		// [UnityEngine.Random.Range (0, brickTypes.Count)];
		Transform[] childs = figure.GetComponentsInChildren<Transform> ();
		foreach (Transform child in childs) {
			if (figure != child.gameObject) {
				child.GetComponent<SpriteRenderer> ().sprite = sprite;
			}
		}
	}

	private GameObject GetNextFigure_Debug ()
	{
		foreach (GameObject figure in figures) {
			if (ContainsName (figure.name, new string[1]{ "Figure_IS" })) {
				return figure;
			}
		}
		throw new Exception ("Figure not found");
	}


	private bool ContainsName (string comparable, string[] values)
	{
		foreach (string value in values) {
			if (comparable.Equals (value)) {
				return true;
			}
		}
		return false;
	}


	public bool Respawn ()
	{
		var figure = GetNextFigure ();
		if (!CanBePlaced (figure, initPosition)) {
			return false;
		}
		GameObject newFigure = Instantiate (figure, new Vector3 (initPosition.x, initPosition.y, 0), Quaternion.identity);
		RandomizeFigureTextures (newFigure);
		newFigure.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, FigureController.FALLING_VELOCITY);

		return true;
	}


	//--------------------------------------------- Board instrument methods -----------------------------


	/*
	 * Get absolute position from board position 
	 */
	public Vector2 getPos (int x, int y)
	{
		Vector2 pos = new Vector2 ();
		pos.x = (x - BOARD_HALF_WIDTH) * BRICK_SIZE;
		pos.y = (-y + BOARD_HALF_HEIGHT - 1) * BRICK_SIZE;
		return pos;
	}


	public Vector2 getPos (Vector2 pos)
	{
		return getPos ((int)pos.x, (int)pos.y);
	}


	/*
	 * Get board position from absolute position
	 */
	private Vector2 GetBoardPos (Vector2 absPos)
	{
		return new Vector2 (BOARD_HALF_WIDTH + absPos.x / BRICK_SIZE, BOARD_HALF_HEIGHT - 1 - absPos.y / BRICK_SIZE);
	}


	//----------------------------------------------------------Check board status--------------------------------------


	private void CheckBoardStatus ()
	{	

		for (int i = BOARD_HEIGHT; i > 0; i--) {
			int counter = 0;

			Vector2 pointA = getPos (0, i);
			Vector2 pointB = getPos (BOARD_WIDTH - 1, i);

			Collider2D[] hits = Physics2D.OverlapAreaAll (pointA, pointB);

			if (BOARD_WIDTH == hits.Length) {
				foreach (Collider2D hit in hits) {
					if (isNotPlayerCollider (hit) && isFigureHaveFalled (hit)) {
						counter++;
					}
				}

				if (counter == BOARD_WIDTH) {
					BurnBrickHelper.instance.BurnBrickLine (hits, dropCoinSfx, treasureBox, CountCoin);
				}

			}
		}
	}

	public delegate void CountCoinDelegate();

	public void CountCoin() {
		coinsCount++;
	}


	private MusicZoneHelper CheckMusicZone ()
	{
		bool isEnergyZoneReached = CheckZoneReached (0, ENERGY_ZONE_HEIGHT);
		bool isCalmZoneReached = CheckZoneReached (0, CALM_ZONE_HEIGHT - 1);
		return new MusicZoneHelper (isEnergyZoneReached, isCalmZoneReached);
	}


	private bool CheckZoneReached (int fromHight, int toHight)
	{
		for (int i = fromHight; i < toHight; i++) {
			Vector2 pointA = getPos (0, i);
			Vector2 pointB = getPos (BOARD_WIDTH - 1, i);
			Collider2D[] hits = Physics2D.OverlapAreaAll (pointA, pointB);
			if (hits.Length != 0) {
				foreach (Collider2D hit in hits) {
					if (isNotPlayerCollider (hit)) {
						return true;
					}
				}
			}
		}
		return false;
	}


	//--------------------------------------- validations --------------------------------------


	private bool isNotPlayerCollider (Collider2D hit)
	{
		return hit.tag != PLAYER_TAG;
	}


	private bool isFigureHaveFalled (Collider2D collider)
	{
		return collider.gameObject.GetComponent<Rigidbody2D> ().velocity.y > FigureController.FALLING_VELOCITY;
	}

	public int GetCoinsCount() {
		return coinsCount;
	}

	public bool CheckAllParticlesFinished ()
	{
		ParticleSystem[] particles = GameObject.FindObjectsOfType<ParticleSystem> ();
		foreach (ParticleSystem particle in particles) {
			if (particle.IsAlive ()) {
				return false;
			}
		}
		return true;
	}

	//---------------------------------------------------------- Figure validations --------------------------------------


	public bool CanRotate (GameObject figure)
	{
		return CanBePlaced (figure, new RotateAction ());
	}

	public bool CanMoveLeft (GameObject figure)
	{
		return CanBePlaced (figure, new MoveLeftAction ());
	}

	public bool CanMoveRight (GameObject figure)
	{
		return CanBePlaced (figure, new MoveRightAction ());
	}

	public bool CanMoveDown (GameObject figure)
	{
		return CanBePlaced (figure, new MoveDownAction ());
	}

	public bool CanNotMoveDown (GameObject figure)
	{
		GameObject figureClone = Instantiate (figure);
		Vector2 pos = figureClone.transform.position;
		pos.y -= MIN_DOWN_DISTANCE;
		figureClone.transform.position = pos;

		if (!CanFigureStay (figureClone)) {
			Destroy (figureClone);
			return true;
		}

		Destroy (figureClone);
		return false;
	}


	public bool CanBePlaced (GameObject figure, Vector2 pos)
	{
		GameObject figureClone = Instantiate (figure);
		figureClone.transform.position = pos;

		if (CanFigureStay (figureClone)) {
			Destroy (figureClone);
			return true;
		}

		Destroy (figureClone);
		return false;
	}


	public bool CanBePlaced (GameObject figure, FigureAction action)
	{

		// first dh

		GameObject figureClone = Instantiate (figure);
		Vector2 pos = figureClone.transform.position;
		pos.y -= MIN_DELTA_H;
		figureClone.transform.position = pos;

		action.setObject (figureClone);
		action.performAction ();

		if (!CanFigureStay (figureClone)) {
			Destroy (figureClone);
			return false;
		}

		// second dh

		pos = figureClone.transform.position;
		pos.y -= (MAX_DELTA_H - MIN_DELTA_H);
		figureClone.transform.position = pos;

		if (!CanFigureStay (figureClone)) {
			Destroy (figureClone);
			return false;
		}

		Destroy (figureClone);
		return true;
	}


	private bool CanFigureStay (GameObject figure)
	{
		Transform[] childs = figure.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform child in childs) {
			if (child.gameObject.GetInstanceID () != figure.GetInstanceID ()) {
				if (!CanBrickStay (child.position)) {
					return false;
				}
			}
		}
		return true;
	}


	public bool CanBrickStay (Vector2 position)
	{
		Vector2 pointA = new Vector2 (position.x - BRICK_HALF_SIZE, position.y + BRICK_HALF_SIZE);
		Vector2 pointB = new Vector2 (position.x + BRICK_HALF_SIZE, position.y - BRICK_HALF_SIZE);

		Collider2D[] hits = Physics2D.OverlapAreaAll (pointA, pointB);
		foreach (Collider2D hit in hits) {
			if (PLAYER_TAG != hit.tag) {
				return false;
			}
		}
		return true;
	}

}
