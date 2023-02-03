import { useTranslation } from 'react-i18next';
import { linkStyle } from "./styles";
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

    const getTimespan = (seconds) => {
        return `${ Math.round(seconds / 60) }:${ String(Math.round(seconds % 60)).padStart(2, '0') }`
    }

    const getStatement = (statement) => {
        return (
            <div>
                <a href={`#T${statement.videoPosition}`} style={linkStyle}>
                    { statement.person }: { getTimespan(statement.durationSeconds) }
                </a>
            </div>
        )
    }

    return (
        <div>
            <h4>{t("Speeches")}</h4>
            <div style={speechesContainerStyle}>
                { statements && statements.map(statement => getStatement(statement)) }
            </div>
            <div style={speechesContainerStyle}>
                { reservations && reservations.map(reservation =>
                    <div style={reservationsRowStyle}>
                        <div style={reservation.active ? linkStyle : null}>
                            {reservation.person}
                        </div>
                        {reservation.active && <FaMicrophone />}
                    </div>)
                }
            </div>
        </div>
    );
}
