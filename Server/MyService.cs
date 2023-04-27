namespace WWWatering
{
    public class MyService
    {
        private string _str;
        private int _counter = 0;
        public MyService()
        {
            _str = "Init";
        }
        public string GetMessage()
        {
            _counter++;
            return _str + _counter.ToString();
        }
    }
}
