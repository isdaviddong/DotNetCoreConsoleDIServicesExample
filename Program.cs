using System;
using Microsoft.Extensions.DependencyInjection;

namespace consoleDI
{
    // 注意，他不是 Main
    public class MainApp
    {
        private readonly ISalaryFormula _SalaryFormula;
        public MainApp(ISalaryFormula SalaryFormula)
        {
            _SalaryFormula = SalaryFormula;
        }
 
        //注意，這裡不是 Console 程式的進入點 Main
        public void Main()
        {
            //一般員工 SalaryFormula
            SalaryCalculator SC = new SalaryCalculator(_SalaryFormula);
            //注意參數完全相同
            float amount = SC.Calculate(8 * 19, 200, 8);
            Console.WriteLine("\nSalaryFormula--->amount:" + amount);
        }
    }

    //這裡才是真正的進入點 main 的類別 Program
    public class Program
    {
        //這裡才是真正的進入點 main
        static void Main(string[] args)
        {
            //建立 DI Services
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            //註冊 DI物件 (為了未來的注入)
            serviceCollection.AddTransient<MainApp>();
            serviceCollection.AddTransient<ISalaryFormula, SalaryFormula>();
            //serviceCollection.AddTransient<ISalaryFormula, BossSalaryFormula>(); //先槓掉
            
            //建立 DI Provider
            var serviceProvider = serviceCollection.BuildServiceProvider();
            //執行 MainApp，這裡會自動注入 ISalaryFormula
            serviceProvider.GetRequiredService<MainApp>().Main();

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// 計算薪資的類別
    /// </summary>
    class SalaryCalculator
    {
        /// <summary>
        /// 計算薪資的公式物件
        /// </summary>
        private ISalaryFormula _SalaryFormula;
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="SalaryFormula"></param>
        public SalaryCalculator(ISalaryFormula SalaryFormula)
        {
            _SalaryFormula = SalaryFormula;
        }
        /// <summary>
        /// 實際計算薪資
        /// </summary>
        /// <param name="WorkHours">工時</param>
        /// <param name="HourlyWage">時薪</param>
        /// <param name="PrivateDayOff">請假天數</param>
        /// <returns></returns>
        public float Calculate(float WorkHours, int HourlyWage, int PrivateDayOffHours)
        {
            return _SalaryFormula.Execute(WorkHours, HourlyWage, PrivateDayOffHours);
        }
    }

    /// <summary>
    /// 預設的計算薪資的公式的類別
    /// </summary>
    class SalaryFormula : ISalaryFormula
    {
        /// <summary>
        /// 實際計算薪資
        /// </summary>
        /// <param name="WorkHours"></param>
        /// <param name="HourlyWage"></param>
        /// <param name="PrivateDayOffHours"></param>
        /// <returns></returns>
        public float Execute(float WorkHours, int HourlyWage, int PrivateDayOffHours)
        {
            //薪資=工時*時薪-(事假時數*時薪)
            return WorkHours * HourlyWage - (PrivateDayOffHours * HourlyWage);
        }
    }

    //老闆薪資計算公式
    class BossSalaryFormula : ISalaryFormula
    {
        public float Execute(float WorkHours, int HourlyWage, int PrivateDayOffHours)
        {
            //老闆請假不扣薪!!!!!!!
            return WorkHours * HourlyWage - (PrivateDayOffHours * HourlyWage * 0);
        }
    }

    /// <summary>
    /// 計算薪資的公式的介面
    /// </summary>
    public interface ISalaryFormula
    {
        //薪資=工時*時薪-(事假時數*時薪)
        float Execute(float WorkHours, int HourlyWage, int PrivateDayOffHours);
    }
}
