using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
// 유니티용 포톤 컴포넌트들
// 포톤 서비스 관련 라이브러리

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class NetworkManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1"; // 게임 버전

    public TextMeshProUGUI connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button joinButton; // 룸 접속 버튼
    public string sceneName;

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start()
    {
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;



        //동기화 필요 클래스 등록
        PhotomCustomTypes.Register();




        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        // 룸 접속 버튼을 잠시 비활성화
        joinButton.interactable = false;
        // 접속을 시도 중임을 텍스트로 표시
        connectionInfoText.text = "Connect to master...";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼을 활성화
        joinButton.interactable = true;
        // 접속 정보 표시
        connectionInfoText.text = "Online : Connected to master";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 룸 접속 버튼을 비활성화
        joinButton.interactable = false;
        // 접속 정보 표시
        connectionInfoText.text = "Offline : Failed to connect to master\nretrying...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        // 중복 접속 시도를 막기 위해, 접속 버튼 잠시 비활성화
        joinButton.interactable = false;
        Debug.Log("Connecting");

        // 마스터 서버에 접속중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 룸 접속 실행
            connectionInfoText.text = "Enter the dungeon...";
            Debug.Log("Enter the dungeon...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // 마스터 서버에 접속중이 아니라면, 마스터 서버에 접속 시도
            connectionInfoText.text = "Offline : Failed to connect to master\nretrying...";
            Debug.Log("Offline : Failed to connect to master\nretrying...");
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 접속 상태 표시
        connectionInfoText.text = "Create new dungeon...";
        // 최대 2명을 수용 가능한 빈방을 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 2});
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        // 접속 상태 표시
        connectionInfoText.text = "Load Completed";
        // 모든 룸 참가자들이 Main 씬을 로드하게 함
        //PhotonNetwork.LoadLevel("Main");
        PhotonNetwork.LoadLevel(sceneName);
    }
}