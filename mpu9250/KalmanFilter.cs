using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mpu9250
{
    /// <summary>
    /// https://stackoverflow.com/questions/39315817/filtering-streaming-data-to-reduce-noise-kalman-filter-c-sharp
    /// </summary>
    public class KalmanFilter
    {
        private double A, H, Q, R, P, x;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initial_noise">Initial noise</param>
        public KalmanFilter(double initial_noise = 0)
        {
            this.A = 1;
            this.H = 1;
            this.Q = 0.125;
            this.R = 1;
            this.P = 1;
            this.x = initial_noise;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="q">Process noise</param>
        /// <param name="initial_noise">Initial noise</param>
        public KalmanFilter(double q = 0.125, double initial_noise = 0)
        {
            this.A = 1;
            this.H = 1;
            this.Q = q;
            this.R = 1;
            this.P = 1;
            this.x = initial_noise;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">Factor of real value to previous real value</param>
        /// <param name="h">The noiseless connection between the state vector and the measurement vector, and is assumed stationary over time</param>
        /// <param name="q">Process noise</param>
        /// <param name="r">Assumed environment noise</param>
        /// <param name="initial_p">Represents initial covariance</param>
        /// <param name="initial_x">Initial noise</param>
        public KalmanFilter(double a = 1, double h = 1, double q = 0.125, double r = 1, double initial_p = 1, double initial_x = 0)
        {
            this.A = a;
            this.H = h;
            this.Q = q;
            this.R = r;
            this.P = initial_p;
            this.x = initial_x;
        }

        public double Smooth(double noisy)
        {
            // time update - prediction
            x = A * x;
            P = A * P * A + Q;

            // measurement update - correction
            var K = P * H / (H * P * H + R);
            x = x + K * (noisy - H * x);
            P = (1 - K * H) * P;

            return x;
        }
    }

    //public class KalmanFilter2
    //{
    //    // assign default values. for a new measurement, reset this values
    //    public double P = double.Parse("1");  // MUST be greater than 0
    //    public double filtered = double.Parse("0"); // any value

    //    public double Smooth(double noisy)
    //    {
    //        // 
    //        double A = double.Parse("1");
    //        // double B = 0; //factor of real value to real control signal
    //        double H = double.Parse("1");
    //        double Q = double.Parse("0.125");
    //        double R = double.Parse("1");

    //        double K;
    //        double z;
    //        double x;

    //        // get current measured value
    //        z = noisy;

    //        // time update - prediction
    //        x = A * filtered;
    //        P = A * P * A + Q;

    //        // measurement update - correction
    //        K = P * H / (H * P * H + R);
    //        x = x + K * (z - H * x);
    //        P = (1 - K * H) * P;

    //        //estimated value
    //        filtered = x;

    //        return filtered;
    //    }
    //}
}
