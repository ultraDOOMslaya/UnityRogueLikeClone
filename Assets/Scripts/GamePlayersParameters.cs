using UnityEngine;

[CreateAssetMenu(fileName = "Players Parameters", menuName = "Scriptable Objects/Game Players Parameters", order = 5)]
public class GamePlayersParameters : GameParameters
{
    public override string GetParametersName() => "Players";

    public PlayerData[] players;
    public int myPlayerId;
}