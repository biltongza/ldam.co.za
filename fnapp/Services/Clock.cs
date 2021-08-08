using System;

namespace ldam.co.za.fnapp.Services
{
    public interface IClock
    {
        DateTime Now();
    }

    public class Clock : IClock
    {
        public DateTime Now() => DateTime.Now;
    }
}