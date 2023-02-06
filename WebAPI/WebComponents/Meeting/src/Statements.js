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

export default function  Statements(props) {
    const { statements, reservations } = props
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
        return `${ Math.round(seconds / 60) }:${ String(Math.round(seconds % 60)).padStart(2, '0') }`
    }

    const getStatement = (statement) => {

        if (statement.isReservation) {
            return getReservation(statement)
        }

        return (
            <div>
                <a href={`#T${statement.videoPosition}`} style={linkStyle}>
                    { statement.person }: { getTimespan(statement.durationSeconds) }
                </a>
            </div>
        )
    }

    const getReservation = (reservation) => {
        return (
            <div style={reservationsRowStyle}>
                <div style={reservation.active ? linkStyle : null}>
                    {reservation.person}
                </div>
                {reservation.active && <FaMicrophone />}
            </div>
        )
    }

    return (
        <div>
            <div style={headingStyle}>{t("Speeches")}</div>
            <div style={speechesContainerStyle}>
                { statementList && statementList.map(statement => getStatement(statement)) }
            </div>
        </div>
    );
}
