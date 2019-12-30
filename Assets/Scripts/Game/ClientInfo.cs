public class ClientInfo
{
    public readonly string roomId;      //readonly 처음에만 값 생성돼고 다음엔 변경이 안됀다.
    public readonly string clientId;

    public ClientInfo(string roomId, string clientId)
    {
        this.roomId = roomId;
        this.clientId = clientId;
    }
}
