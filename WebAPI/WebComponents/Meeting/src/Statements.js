import { useTranslation } from 'react-i18next';
import { headingStyle, linkStyle } from "./styles";

const speechesContainerStyle = {
    columnCount: 2,
    columnGap: "10px",
    boxSizing: "border-box",
    fontSize: "1em"
}

export default function  Statements(props) {
    const { statements } = props
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
            <div style={headingStyle}>{t("Speeches")}</div>
            <div style={speechesContainerStyle}>
                { statements && statements.map(statement => getStatement(statement)) }
            </div>
        </div>
    );
}



