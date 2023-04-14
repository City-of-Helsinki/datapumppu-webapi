import { useTranslation } from 'react-i18next';
import { headingStyle, linkStyle } from "./styles";
import { FaMicrophone } from "react-icons/fa";

const speechesContainerStyle = {
    columnCount: 2,
    columnGap: "10px",
    boxSizing: "border-box",
    fontSize: "1em"
}

const reservationsRowStyle = {
    display: "flex",
    flexDirection: "row",
    alignItems: "center"    
}

const subItemHeaderStyle = {
    marginTop: "1rem",
    backgroundColor: "#666666",
    color: "#ffffff",
    padding: "8px",
}

export default function  Statements(props) {
    const { statements, reservations, itemNumber, itemTextFi } = props
    const { t } = useTranslation();

    const getStatementArray = () => {
        return statements ?? []
    }

    const getReservationArray = () => {
        return reservations ?? []
    }

    
    const statementList = getStatementArray()
        .concat(getReservationArray().map(r => ({...r, isReservation: true})))


    const getTimespan = (seconds) => {
        return `${ Math.floor(seconds / 60) }:${ String(Math.round(seconds % 60)).padStart(2, '0') }`
    }

    const getAdditionalInfo = (statement) => {
        if ("fi" === "#--LANGUAGE--#".toLowerCase()) {
            return statement?.additionalInfoFI
        } else {
            return statement?.additionalInfoSV
        }
    }

    const getStatement = (statement) => {

        if (!statement) {
            return;
        }

        if (statement?.isReservation) {
            return getReservation(statement)
        }

        return (
            <div>
                <a href={`#T${statement?.videoPosition}`} style={linkStyle}>
                    { `${statement?.person} (${getAdditionalInfo(statement)}) ${getTimespan(statement?.durationSeconds ?? 0)}` }
                </a>
            </div>
        )
    }

    const getReservation = (reservation) => {
        return (
            <div style={reservationsRowStyle}>
                <div style={reservation.active ? linkStyle : null}>
                    {`${reservation.person} (${getAdditionalInfo(statement)}) `}
                </div>
                {reservation.active && <FaMicrophone />}
            </div>
        )
    }

    return (
        <div>
            {itemNumber && <div style={subItemHeaderStyle}>{`${itemNumber} ${itemTextFi}`}</div>}
            <div style={headingStyle}>{t("Speeches")}</div>
            <div style={speechesContainerStyle}>
                { statementList && statementList.map(statement => getStatement(statement)) }
            </div>
        </div>
    );
}
