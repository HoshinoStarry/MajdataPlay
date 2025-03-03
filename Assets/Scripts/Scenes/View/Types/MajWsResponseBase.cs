namespace MajdataPlay.View.Types
{
    internal readonly struct MajWsResponseBase
    {
        public MajWsResponseType responseType { get; init; }
        public object responseData { get; init; }
    }
    public enum MajWsResponseType
    {
        Error = 400,
        Ok = 200,
        PlayStarted = 201
    }
}