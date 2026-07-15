using EDDY.IS.MatchingEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public enum MatchItemSortType
    {
        None,
        FailedValidationPaidStatusType,
        RankScore,
        SAB
    }

    public static class MatchItemComparerFactory
    {
        public static IComparer<MatchItem> Create(MatchItemSortType sortType)
        {
            IComparer<MatchItem> comparer = null;

            switch (sortType)
            {
                case MatchItemSortType.None:
                    break;
                case MatchItemSortType.FailedValidationPaidStatusType:
                    comparer = new MatchItemComparerStandard(false);
                    break;
                case MatchItemSortType.RankScore:
                    comparer = new MatchItemComparerStandard(true);
                    break;
                case MatchItemSortType.SAB:
                    comparer = new MatchItemComparerSAB();
                    break;
                default:
                    break;
            }

            return comparer;
        }
    }

    public class MatchItemComparerSAB : IComparer<MatchItem>
    {
        public MatchItemComparerSAB()
        { }

        public int Compare(MatchItem item1, MatchItem item2)
        {
            if (item1.FailedValidation == item2.FailedValidation)
            {
                if (item1.Match.PaidStatusTypeId == item2.Match.PaidStatusTypeId || (item1.Match.SFProductCode_PSI == SFProductCode.SAB_HB && item2.Match.SFProductCode_PSI == SFProductCode.SAB_HB))
                {
                    if (item1.Match.SFProductCode_PSI == item2.Match.SFProductCode_PSI)
                    {
                        if (item1.Match.SFProductCode_PSI == SFProductCode.SAB_TOP)
                        {
                            if (item1.Match.SABSRAPosition_PSI == item2.Match.SABSRAPosition_PSI)
                            {
                                if (item1.ProgramRankScore == item2.ProgramRankScore)
                                    return 0;
                                else if (item1.ProgramRankScore > item2.ProgramRankScore)
                                    return -1;
                                else
                                    return 1;
                            }
                            else if (item1.Match.SABSRAPosition_PSI.HasValue && item2.Match.SABSRAPosition_PSI.HasValue)
                            {
                                if (item1.Match.SABSRAPosition_PSI.Value > item2.Match.SABSRAPosition_PSI.Value)
                                    return 1;
                                else
                                    return -1;
                            }
                            else if (item1.Match.SABSRAPosition_PSI.HasValue)
                                return -1;
                            else
                                return 1;
                        }
                        else if (item1.Match.SFProductCode_PSI == SFProductCode.SAB_CPL || item1.Match.SFProductCode_PSI == SFProductCode.SAB_CPC || item1.Match.SFProductCode_PSI == SFProductCode.SAB_MS)
                        {
                            if (item1.Match.SABSRAPosition_CR == item2.Match.SABSRAPosition_CR)
                            {
                                if (item1.ProgramRankScore == item2.ProgramRankScore)
                                    return 0;
                                else if (item1.ProgramRankScore > item2.ProgramRankScore)
                                    return -1;
                                else
                                    return 1;
                            }
                            else if (item1.Match.SABSRAPosition_CR.HasValue && item2.Match.SABSRAPosition_CR.HasValue)
                            {
                                if (item1.Match.SABSRAPosition_CR.Value > item2.Match.SABSRAPosition_CR.Value)
                                    return 1;
                                else
                                    return -1;
                            }
                            else if (item1.Match.SABSRAPosition_CR.HasValue)
                                return -1;
                            else
                                return 1;
                        }
                        else
                        {
                            if (item1.ProgramRankScore == item2.ProgramRankScore)
                                return 0;
                            else if (item1.ProgramRankScore > item2.ProgramRankScore)
                                return -1;
                            else
                                return 1;
                        }
                    }
                    else if (item1.Match.SFProductCode_PSI.HasValue && item2.Match.SFProductCode_PSI.HasValue)
                    {
                        if (item1.Match.PaidStatusTypeId == PaidStatusType.Fraid && item1.Match.SFProductCode_PSI == SFProductCode.SAB_HB)
                            return -1;
                        if (item2.Match.PaidStatusTypeId == PaidStatusType.Fraid && item2.Match.SFProductCode_PSI == SFProductCode.SAB_HB)
                            return 1;
                        if (item1.Match.SFProductCode_PSI.Value > item2.Match.SFProductCode_PSI.Value)
                            return 1;
                        else
                            return -1;
                    }
                    else if (item1.Match.SFProductCode_PSI.HasValue)
                        return -1;
                    else
                        return 1;
                }
                else
                {
                    if (item1.Match.PaidStatusTypeId > item2.Match.PaidStatusTypeId)
                    {
                        if (item2.Match.SFProductCode_PSI == SFProductCode.SAB_HB && item1.Match.SFProductCode_PSI == SFProductCode.SAB_SL)
                            return 1;
                        else
                            return -1;
                    }
                    else
                    {
                        if (item1.Match.SFProductCode_PSI == SFProductCode.SAB_HB && item2.Match.SFProductCode_PSI == SFProductCode.SAB_SL)
                            return -1;
                        else
                            return 1;
                    }
                }
            }
            else if (item1.FailedValidation == false)
                return -1;
            else
                return 1;
        }
    }


    public class MatchItemComparerStandard : IComparer<MatchItem>
    {
        private bool _useProgramRankScore;
        public MatchItemComparerStandard()
        { }

        public MatchItemComparerStandard(bool useProgramRankScore)
        {
            _useProgramRankScore = useProgramRankScore;
        }
        
        public int Compare(MatchItem item1, MatchItem item2)
        {
            if (item1.FailedValidation == item2.FailedValidation)
            {
                if (item1.Match.PaidStatusTypeId == item2.Match.PaidStatusTypeId)
                {
                    if (item1.Match.SFProductCode_PSI.HasValue == item2.Match.SFProductCode_PSI.HasValue)
                    {
                        if (_useProgramRankScore)
                        {
                            if (item1.ProgramRankScore == item2.ProgramRankScore)
                                return 0;
                            else if (item1.ProgramRankScore > item2.ProgramRankScore)
                                return -1;
                            else
                                return 1;
                        }
                        else
                            return 0;
                    }
                    else if (item1.Match.SFProductCode_PSI.HasValue)
                        return 1;
                    else
                        return -1;                    
                }
                else if (item1.Match.PaidStatusTypeId > item2.Match.PaidStatusTypeId)
                    return -1;
                else
                    return 1;
            }
            else if (item1.FailedValidation == false)
                return -1;
            else
                return 1;
        }
    }  

    public class InstitutionComparerSAB : IComparer<Institution>
    {
        public InstitutionComparerSAB()
        { }

        public int Compare(Institution item1Initial, Institution item2Initial)
        {
            InstitutionWithProgram item1 = (InstitutionWithProgram)item1Initial;
            InstitutionWithProgram item2 = (InstitutionWithProgram)item2Initial;

            if (item1.FailedValidation == item2.FailedValidation)
            {
                if (item1.ProgramList[0].PaidStatusTypeId == item2.ProgramList[0].PaidStatusTypeId || (item1.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_HB && item2.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_HB))
                {
                    //we need this line in order to treat sab_cpc as sab_cpl when sorting. This is because sab_cpc is id 8 and cpl is 2 so its needed for sorting purposes to treat them the same
                    SFProductCode? sfproductone = item1.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_CPC ? SFProductCode.SAB_CPL : item1.ProgramList[0].ProgramSFProductCode;
                    SFProductCode? sfproducttwo = item2.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_CPC ? SFProductCode.SAB_CPL : item2.ProgramList[0].ProgramSFProductCode;
                    if (sfproductone == sfproducttwo)
                    {
                        if (sfproductone == SFProductCode.SAB_TOP)
                        {
                            if (item1.ProgramList[0].SABSRAPosition_PSI == item2.ProgramList[0].SABSRAPosition_PSI)
                            {
                                if (item1.ProgramRankScore == item2.ProgramRankScore)
                                    return 0;
                                else if (item1.ProgramRankScore > item2.ProgramRankScore)
                                    return -1;
                                else
                                    return 1;
                            }
                            else if (item1.ProgramList[0].SABSRAPosition_PSI.HasValue && item2.ProgramList[0].SABSRAPosition_PSI.HasValue)
                            {
                                if (item1.ProgramList[0].SABSRAPosition_PSI.Value > item2.ProgramList[0].SABSRAPosition_PSI.Value)
                                    return 1;
                                else
                                    return -1;
                            }
                            else if (item1.ProgramList[0].SABSRAPosition_PSI.HasValue)
                                return -1;
                            else
                                return 1;
                        }
                        else if (sfproductone == SFProductCode.SAB_CPL)
                        {
                            if (item1.SABSRAPosition_CR == item2.SABSRAPosition_CR)
                            {
                                if (item1.ProgramRankScore == item2.ProgramRankScore)
                                    return 0;
                                else if (item1.ProgramRankScore > item2.ProgramRankScore)
                                    return -1;
                                else
                                    return 1;
                            }
                            else if (item1.SABSRAPosition_CR.HasValue && item2.SABSRAPosition_CR.HasValue)
                            {
                                if (item1.SABSRAPosition_CR.Value > item2.SABSRAPosition_CR.Value)
                                    return 1;
                                else
                                    return -1;
                            }
                            else if (item1.SABSRAPosition_CR.HasValue)
                                return -1;
                            else
                                return 1;
                        }
                        else
                        {
                            if (item1.ProgramRankScore == item2.ProgramRankScore)
                                return 0;
                            else if (item1.ProgramRankScore > item2.ProgramRankScore)
                                return -1;
                            else
                                return 1;
                        }
                    }
                    else if (sfproductone.HasValue && sfproducttwo.HasValue)
                    {
                        if (item1.ProgramList[0].PaidStatusTypeId == PaidStatusType.Fraid && sfproductone == SFProductCode.SAB_HB)
                            return -1;
                        if (item2.ProgramList[0].PaidStatusTypeId == PaidStatusType.Fraid && sfproducttwo == SFProductCode.SAB_HB)
                            return 1;
                        if (sfproductone > sfproducttwo)
                            return 1;
                        else
                            return -1;
                    }
                    else if (sfproductone.HasValue)
                        return -1;
                    else
                        return 1;
                }
                else
                {
                    if (item1.ProgramList[0].PaidStatusTypeId > item2.ProgramList[0].PaidStatusTypeId)
                    {
                        if (item2.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_HB && item1.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_SL)
                            return 1;
                        else
                            return -1;
                    }
                    else
                    {
                        if (item1.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_HB && item2.ProgramList[0].ProgramSFProductCode == SFProductCode.SAB_SL)
                            return -1;
                        else
                            return 1;
                    }
                }
            }
            else if (item1.FailedValidation == false)
                return -1;
            else
                return 1;
        }
    }

    public class InstitutionComparerStandard : IComparer<Institution>
    {
        private GetInstitutionCampusOption _campusOption;
        private bool _isValidGeoTarget;
        private int _applicationId;

        public InstitutionComparerStandard()
        { }

        public InstitutionComparerStandard(GetInstitutionCampusOption campusOption, bool isValidGeoTarget, int applicationId)
        {
            this._campusOption = campusOption;
            this._isValidGeoTarget = isValidGeoTarget;
            this._applicationId = applicationId;
        }

        public int Compare(Institution item1Initial, Institution item2Initial)
        {
            if (item1Initial.FailedValidation == item2Initial.FailedValidation)
            {
                #region equal failedvalidation

                Program item1Program = _campusOption == GetInstitutionCampusOption.CampusOn2ndLevel ? ((InstitutionWithCampus)item1Initial).CampusList[0].ProgramList[0] : ((InstitutionWithProgram)item1Initial).ProgramList[0];
                Program item2Program = _campusOption == GetInstitutionCampusOption.CampusOn2ndLevel ? ((InstitutionWithCampus)item2Initial).CampusList[0].ProgramList[0] : ((InstitutionWithProgram)item2Initial).ProgramList[0];

                if (item1Program.PaidStatusTypeId == item2Program.PaidStatusTypeId)
                {
                    if (item1Program.ProgramSFProductCode.HasValue == item2Program.ProgramSFProductCode.HasValue)
                    {
                        #region equal SFProductCode

                        if (_isValidGeoTarget && _applicationId == 7)
                        {
                            if (item1Initial.IsGeotargeted == item2Initial.IsGeotargeted)
                            {
                                if (item1Initial.ProgramRankScore == item2Initial.ProgramRankScore)
                                    return 0;
                                else if (item1Initial.ProgramRankScore > item2Initial.ProgramRankScore)
                                    return -1;
                                else
                                    return 1;
                            }
                            else if (item1Initial.IsGeotargeted)
                                return -1;
                            else
                                return 1;                                
                        }
                        else
                        {
                            if (item1Initial.ProgramRankScore == item2Initial.ProgramRankScore) 
                                return 0;
                            else if (item1Initial.ProgramRankScore > item2Initial.ProgramRankScore)
                                return -1;
                            else
                                return 1;
                        }

                        #endregion
                    }
                    else if (item1Program.ProgramSFProductCode.HasValue)
                        return 1;
                    else
                        return -1;
                }
                else if (item1Program.PaidStatusTypeId > item2Program.PaidStatusTypeId)
                    return -1;
                else
                    return 1;

                #endregion
            }
            else if (item1Initial.FailedValidation == false)
                return -1;
            else
                return 1;
        }
    }

    public class InstitutionComparerGS : IComparer<Institution>
    {
        private GetInstitutionCampusOption _campusOption;
        private bool _isValidGeoTarget;
        private int _applicationId;

        public InstitutionComparerGS()
        { }

        public InstitutionComparerGS(GetInstitutionCampusOption campusOption, bool isValidGeoTarget, int applicationId)
        {
            this._campusOption = campusOption;
            this._isValidGeoTarget = isValidGeoTarget;
            this._applicationId = applicationId;
        }

        public int Compare(Institution item1Initial, Institution item2Initial)
        {
            Program item1Program = _campusOption == GetInstitutionCampusOption.CampusOn2ndLevel ? ((InstitutionWithCampus)item1Initial).CampusList[0].ProgramList[0] : ((InstitutionWithProgram)item1Initial).ProgramList[0];
            Program item2Program = _campusOption == GetInstitutionCampusOption.CampusOn2ndLevel ? ((InstitutionWithCampus)item2Initial).CampusList[0].ProgramList[0] : ((InstitutionWithProgram)item2Initial).ProgramList[0];
                          
            if (item1Program.PaidStatusTypeId == item2Program.PaidStatusTypeId)
            {
                if (item1Initial.FailedValidation == item2Initial.FailedValidation)
                {
                    if (item1Program.ProgramSFProductCode.HasValue == item2Program.ProgramSFProductCode.HasValue)
                    {
                        if (_isValidGeoTarget && _applicationId == 7)
                        {
                            if (item1Initial.IsGeotargeted == item2Initial.IsGeotargeted)
                            {
                                if (item1Initial.ProgramRankScore == item2Initial.ProgramRankScore)
                                    return 0;
                                else if (item1Initial.ProgramRankScore > item2Initial.ProgramRankScore)
                                    return -1;
                                else
                                    return 1;
                            }
                            else if (item1Initial.IsGeotargeted)
                                return -1;
                            else
                                return 1;
                        }
                        else
                        {
                            if (item1Initial.ProgramRankScore == item2Initial.ProgramRankScore)
                                return 0;
                            else if (item1Initial.ProgramRankScore > item2Initial.ProgramRankScore)
                                return -1;
                            else
                                return 1;
                        }
                    }
                    else if (item1Program.ProgramSFProductCode.HasValue)
                        return 1;
                    else
                        return -1;
                }
                else if (item1Initial.FailedValidation == false)
                    return -1;
                else
                    return 1;
            }
            else if (item1Program.PaidStatusTypeId > item2Program.PaidStatusTypeId)
                return -1;
            else
                return 1;           
        }
    }
}
