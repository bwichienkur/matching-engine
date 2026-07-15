using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
	internal class LiveTransferNumberProcessor
	{
        public static string GetLiveTransferNumber(MatchItem mi, DTO.ProspectInput prospect)
        {
            bool isOnline = mi.Match.ProgramCampusTypeId == 1;
            var group = "";
            bool isGraduate = (mi.Match.ProgramLevelId == 7 || mi.Match.ProgramLevelId == 8);
            bool? isMilitary = null;
            State? st = null;

            if (prospect != null && prospect.IsMilitary.HasValue)
                isMilitary = prospect.IsMilitary.Value;
            else if (prospect != null && prospect.MilitaryStatusId.HasValue)
                isMilitary = (prospect.MilitaryStatusId.Value != 126);

            if (prospect != null && prospect.StateId.HasValue)
            {
                if (Enum.IsDefined(typeof(State), prospect.StateId.Value))
                    st = (State)(prospect.StateId.Value);
            }

            switch (mi.Match.InstitutionId)
            {
                case 28: //argosy
                    if (isGraduate)
                        group = GetHuntGroup(mi);
                    
                    if (isMilitary.HasValue && isMilitary.Value && st.HasValue && st.Value == State.Alabama)
                    {
                        if (group == "5_WK")
                            return "(855)827-5865";
                        else if (group == "7.5_WK")
                            return "(855)846-7781";
                        else
                            return "(480)327-1223";
                    }
                    else if (isMilitary.HasValue && isMilitary.Value)
                    {
                        if (group == "5_WK")
                            return "(855)827-5864";
                        else if (group == "7.5_WK")
                            return "(855)846-7725";
                        else
                            return "(855)784-1266";
                    }
                    else if (st.HasValue && st.Value == State.Alabama)
                    {
                        if (group != "")
                            return "(855)827-5861";
                        else
                            return "(480)327-1221";
                    }
                    else
                    {
                        if (group != "")
                            return "(855)846-7723";
                        else
                            return "(855)784-1264";
                    }
                    break;
                case 4696: //University of Texas at Arlington
                    group = GetHuntGroup(mi);
                    if (group == "FNP_DNP_MSN")
                        return "(844)-351-9395";
                    else if (group == "BSN_RNTOBSN_RNTOMSN")
                        return "(844)-378-4035";
                    break;
                case 5218: //bryanu
                    group = GetHuntGroup(mi);
                    if (group == "Legal")
                        return "(480)739-3628";
                    else if (group == "Business")
                        return "(480)725-5427";
                    else if (group == "Fitness")
                        return "(480)725-2027";
                    else if (group == "Health")
                        return "(480)771-1280";
                    break;
                case 5712: //trident
                    group = GetHuntGroup(mi);
                    if (group == "PhD")
                        return "(844)811-5142";
                    else
                        return "(714)644-8111";
                    break;
                case 4967: //wentworth
                    group = GetHuntGroup(mi);
                    if (group == "BS_ConstructionManagement")
                        return "(617)989-4414";
                    else if (group == "Management")
                        return "(617)989-4409";
                    break;
                case 5471: //auguste culinary
                    if (mi.Match.ProgramId == 186379)
                        return "(512)381-8590";
                    else if (mi.Match.ProgramId == 186380)
                        return "(512)381-8587";
                    else if (mi.Match.ProgramId == 186381)
                        return "(512)381-8587";
                    break;
                case 316: //Post University
                    if (isMilitary.HasValue && isMilitary.Value)
                        return "(203)596-6141";
                    else
                        return "(203)596-6081";
                    break;
                case 5545: //Independence University
                    if (mi.Match.ProductId == 52) //WTT
                        return "(800)478-1332";
                    else if (mi.Match.ProductId == 120) //LTT2
                        return "(800)794-9617";
                    else
                        return "(855)343-4744";
                case 357: //South University
                    group = GetHuntGroup(mi);
                    //Returns: Nursing or Default
                    if (mi.Match.ProgramId == 311834)
                        return "(912)201-6547";
                    else if (isMilitary.HasValue && isMilitary.Value)
                    {
                        if (group == "Nursing")
                            return "(912)201-6544";
                        else if (isGraduate)
                            return "(912)201-6546";
                        else
                            return "(912)201-6545";
                    }
                    else
                    { 
                        if (group == "Nursing")
                            return "(912)201-6542";
                        else if (isGraduate)
                            return "(912)201-6541";
                        else
                            return "(912)201-6543";
                    }
                case 256: //Ancora && MMC
                    group = GetHuntGroup(mi);
                    if (group == "PharmTech")
                        return "(866)708-8737";
                    else if (group == "IT_Programs")
                        return "(623)745-3552";
                    else if (group == "Healthcare_Programs")
                        return "(800)705-5369";
                    else if (group == "Business_Programs")
                        return "(800)761-5843";
                    break;
            }

            return mi.Match.PhoneNumber;
        }

        private static string GetHuntGroup(MatchItem mi)
        {
            switch (mi.Match.InstitutionId)
            {
                case 28: // Argosy
                    switch (mi.Match.ProgramId)
                    {
                        case 3893:
                        case 3903:
                        case 4232:
                        case 4233:
                        case 20922:
                        case 20923:
                        case 20924:
                        case 20925:
                        case 21294:
                        case 185303:
                        case 185304:
                        case 192491:
                        case 296737:
                        case 297914:
                            return "5_WK";
                        case 3284:
                        case 3285:
                        case 3286:
                        case 3287:
                        case 3293:
                        case 3302:
                        case 3895:
                        case 3907:
                        case 3917:
                        case 4234:
                        case 21074:
                        case 22112:
                        case 22115:
                        case 22117:
                        case 22152:
                        case 22153:
                        case 192490:
                        case 192823:
                            return "7.5_WK";

                    }
                    break;
                case 4696: //University of Texas at Arlington
                    switch (mi.Match.ProgramId)
                    {
                        case 294751:
                        case 294749:
                        case 294750:
                        case 294746:
                        case 294748:
                        case 294747:
                        case 295366:
                        case 295367:
                            return "FNP_DNP_MSN";
                        case 295370:
                        case 295368:
                        case 295369:
                        case 195019:
                            return "BSN_RNTOBSN_RNTOMSN";
                    }
                    break;
                case 5218: //bryanu
                    switch (mi.Match.ProgramId)
                    {
                        case 273006:
                            return "Legal";
                        case 294878:
                            return "Business";
                        case 273001:
                        case 273002:
                            return "Fitness";
                        case 273005:
                        case 273004:
                            return "Health";
                    }
                    break;
                case 5712: //trident
                    switch (mi.Match.ProgramLevelId)
                    {
                        case 7:
                            return "PhD";
                        default:
                            return "Standard";
                    }
                case 4967: //wentworth
                    switch (mi.Match.ProgramId)
                    {
                        case 297850:
                        case 297851:
                            return "BS_ConstructionManagement";
                        case 297852:
                        case 297854:
                        case 297855:
                        case 297856:
                        case 297858:
                        case 297860:
                        case 297877:
                        case 297853:
                        case 297857:
                        case 297859:
                        case 297861:
                        case 297862:
                        case 297863:
                            return "Management";
                        default:
                            return "Standard";
                    }
                case 256:
                    if(mi.Match.ClientId == 5717)
                        {
                        switch (mi.Match.ProgramId)
                        {
                            case 308540:
                                return "PharmTech";
                            default:
                                return "Default";
                        }
                    }
                    if (mi.Match.ClientId == 8690)
                    {
                        switch (mi.Match.ProgramId)
                        {
                            case 196540:
                            case 292277:
                            case 306960:
                                return "IT_Programs";
                            case 2599:
                            case 2600:
                            case 196534:
                            case 196538:
                            case 196541:
                            case 295186:
                            case 306961:
                            case 308540:
                            case 312997:
                                return "Healthcare_Programs";
                            case 2598:
                            case 196533:
                            case 196535:
                            case 196539:
                            case 295185:
                            case 295200:
                            case 308539:
                            case 310711:
                            case 312998:
                                return "Business_Programs";
                            default:
                                return "Default";
                        }
                    }
                    else
                        return "Default";
                case 357: //South University
                    switch (mi.Match.ProgramLevelId)
                    {
                        case 310542:
                        case 185753:
                        case 185754:
                        case 185755:
                        case 310543:
                        case 185242:
                        case 197907:
                        case 197908:
                        case 197909:
                        case 197910:
                        case 310545:
                        case 272399:
                        case 272400:
                        case 197914:
                        case 197915:
                        case 197916:
                            return "Nursing";
                        default:
                            return "Default";
                    }
            }

            return "";
        }
	}
}
