using System.ComponentModel;

namespace MOD4.Web.Enum
{
    public enum DepartmentEnum
    {

        [Description("3F 廠長")]
        FactoryDirector3 = 1,
        [Description("2F 廠長")]
        FactoryDirector2 = 2,
        [Description("製造工程一部")]
        MFG1 = 3,
        [Description("製造工程二部")]
        MFG2 = 4,
        [Description("支援一部")]
        SUP1 = 5,
        [Description("製造工程三部")]
        MFG3 = 6,
        [Description("製造工程四部")]
        MFG4 = 7,
        [Description("支援二部")]
        SUP2 = 8,
        [Description("製造工程一部工程一課")]
        MFG1ENG1 = 9,
        [Description("製造工程一部工程二課")]
        MFG1ENG2 = 10,
        [Description("製造工程一部工程三課")]
        MFG1ENG3 = 11,
        [Description("製造工程一部工程四課")]
        MFG1ENG4 = 12,
        [Description("製造工程二部工程一課")]
        MFG2ENG1 = 13,
        [Description("製造工程二部工程二課")]
        MFG2ENG2 = 14,
        [Description("製造工程二部工程三課")]
        MFG2ENG3 = 15,
        [Description("製造工程二部工程四課")]
        MFG2ENG4 = 16,
        [Description("支援一部支援一課")]
        SUP1Section1 = 17,
        [Description("支援一部支援二課")]
        SUP1Section2 = 18,
        [Description("製造工程三部工程一課")]
        MFG3ENG1 = 19,
        [Description("製造工程三部工程二課")]
        MFG3ENG2 = 20,
        [Description("製造工程四部工程一課")]
        MFG4ENG1 = 21,
        [Description("製造工程四部工程二課")]
        MFG4ENG2 = 22,
        [Description("支援二部支援一課")]
        SUP2Section1 = 23,
        [Description("支援二部支援二課")]
        SUP2Section2 = 24,
        [Description("量產整合工程")]
        MassIntegrate = 25,
        [Description("新產品整合工程")]
        NewIntegrate = 26,
        [Description("量產整合工程整合一課")]
        MassIntegrate1 = 27,
        [Description("量產整合工程整合二課")]
        MassIntegrate2 = 28,
        [Description("量產整合工程整合三課")]
        MassIntegrate3 = 29,
        [Description("新產品整合工程整合一課")]
        NewIntegrate1 = 30,
        [Description("新產品整合工程整合二課")]
        NewIntegrate2 = 31,
        [Description("新產品整合工程整合三課")]
        NewIntegrate3 = 32,
        [Description("新產品整合工程整合工程部")]
        NewIntegrateENG = 33,
        [Description("製造工程三部製造課")]
        MFG3MFG = 34,
        [Description("製造工程二部製造課")]
        MFG2MFG = 35,
        [Description("製造工程一部製造課")]
        MFG31MFG = 36,
        [Description("品質管理處")]
        QualityControl = 37,
        [Description("品質管理處品質保證一課")]
        QCPromise1 = 38,
        [Description("品質管理處品質保證二課")]
        QCPromise2 = 39,
        [Description("品質管理處品質保證三課")]
        QCPromise3 = 40,
        [Description("品質管理處品質保證四課")]
        QCPromise4 = 41,
        [Description("品質管理處製造品質整合課")]
        QCMFGIntegrate = 42,
        [Description("產能效能部")]
        Performance = 43,
        [Description("產能效能部效能一課")]
        PerformanceEfficacy1 = 44,
        [Description("產能效能部效能二課")]
        PerformanceEfficacy2 = 45,
        [Description("環安處")]
        Environmental = 46,
        [Description("環安處環安二部")]
        EnvDept2 = 47,
        [Description("環安二部台灣環安課")]
        EnvDept2TaiwanEnv = 48,
        [Description("設備製造部")]
        EquipmentMFG = 49,
        [Description("設備製造部電控組裝課")]
        EqMFGASSY = 50,
        [Description("品質系統稽核部")]
        QualityAudit = 51,
        [Description("品質系統稽核部品質稽核課")]
        QASectionQA = 52,
        [Description("品質系統處")]
        QualityAuditSystem = 53,
        [Description("品質系統處流程認證課")]
        QASysProcessSection = 54,
        [Description("模組整合處")]
        ModuleIntegrate = 55,
        [Description("製造工程三部製造課")]
        MIIntegrate4 = 56,
        [Description("模組整合處")]
        MIMI = 57,
    }
}
