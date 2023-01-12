import { useState } from "react";
import { useTranslation } from 'react-i18next';

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
        return `${ Math.round(seconds / 60) }:${ Math.round(seconds % 60) }`
    }

    const getStatement = (statement) => {
        return (
            <div>
                <a href={`#T${statement.videoPosition}`}>
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
        </div>
    );
}



