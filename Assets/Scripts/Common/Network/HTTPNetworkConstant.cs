public class HTTPNetworkConstant
{
    //public const string serverURL = "https://tictactoe-goya.herokuapp.com";     //선생님 서버
    //public const string serverURL = "https://tictactoe-ok.herokuapp.com/";     //내 서버
    public const string serverURL = "localhost:3000";

    //POST
    public const string logInRequestURL = "/users/login";       //로그인
    //public const string logInRequestURL = "/users/signin";       //로그인
    public const string signUpRequestURL = "/users/signup";     //회원가입
    public const string addScoreRequestURL = "/users/addscore"; //스코어 저장
    public const string addMessageRequestURL = "/chat/add";     //메세지 저장

    //GET
    public const string infoRequestURL = "/users/info";
    public const string logoutURL = "/users/logout";            //로그아웃
    public const string chatRequestURL = "/chat/";              //메세지 받기
}
