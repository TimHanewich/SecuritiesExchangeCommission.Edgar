using System;

namespace SecuritiesExchangeCommission.Edgar
{
    public enum TransactionType
    {
        OpenMarketOrPrivatePurchase = 0, //P
        OpenMarketOrPrivateSale = 1, //S
        TransactionVoluntarilyReportedEarlierThanRequired = 2, //V
        GrantOrAward = 3, //A
        SaleBackToIssuer = 4, //D
        PaymentOfExercisePriceOrTaxLiability = 5, //F
        DiscretionaryTransaction = 6, //I
        ExerciseOrConversionOfDerivativeSecurity = 7, //M
        ConversionOfDerivativeSecurity = 8, //C
        ExpirationOfShortDerivativePosition = 9, //E
        ExpirationOfLongDerivativePosition = 10, //H
        ExerciseOfOutOfMoneyDerivative = 11, //O
        ExerciseOfInMoneyDerivative = 12, //X
        BonaFideGift = 13, //G
        SmallAcquisition = 14, //L
        AcquisitionOrDispositionByWillOrLaws = 15, //W
        DepositIntoOrWithdrawalFromVotingTrust = 16, //Z
        OtherAcquisitionOrDisposition = 17, //J
        TransactionInEquitySwap = 18, //K
        DispositionDueToTenderOfShares = 17 //U
    }
}