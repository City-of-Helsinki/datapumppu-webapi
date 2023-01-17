import { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next';
import SeatMap from './SeatMap'
import Voting from './Voting'
import Statements from './Statements'
import { iconStyle, itemStyle, itemOpenStyle } from './styles';
import EditableItem from './EditableItem';

const contentStyle = {
    paddingTop: "15px"
}

const agendaTitleButtonStyle = {
    backgroundColor: "inherit",
    border: "none",
    fontWeight: "bold",
    textAlign: "start",
    padding: "8px 8px 6px 8px",
    color: "black"
}

export default function AgendaItem(props) {
    const [accordionOpen, setAccordionOpen] = useState(false)
    const [showSeatMap, setShowSeatMap] = useState(false)
    const [voting, setVoting] = useState(undefined)
    const [statements, setStatements] = useState(undefined)
    const { agenda, index, meetingId, decision, editable } = props
    const { t } = useTranslation();

    useEffect(() => {
        const fetchVotingData = async () => {
            const response = await fetch(`#--API_URL--#/voting/${meetingId}/${agenda.agendaPoint}`)
            if (response.status === 200) {
                const data = await response.json()
                setVoting(data)
            }
        }

        const fetchStatementsData = async () => {
            const response = await fetch(`#--API_URL--#/statement/${meetingId}/${agenda.agendaPoint}`)
            if (response.status === 200) {
                const data = await response.json()
                setStatements(data)
            }
        }

        if (accordionOpen === true && voting === undefined) {
            fetchStatementsData()
            if (voting === undefined) {
                fetchVotingData()
            }
        }
    }, [accordionOpen])

    const decisionResolutionText = t('Decision resolution')
    const decisionText = t('Decision')
    const openText = t('Open')
    const motionPath = `https://paatokset.hel.fi/#--LANGUAGE--#/asia/${agenda?.caseIDLabel?.replace(" ", "-")}#`
    const decisionPath = `https://paatokset.hel.fi/#--LANGUAGE--#/asia/${decision?.caseID}?paatos=${decision?.nativeId.replace("/[{}]/g", "")}`
    return (
        <div style={accordionOpen ? itemOpenStyle : itemStyle}>
            <button style={agendaTitleButtonStyle} onClick={() => setAccordionOpen(!accordionOpen)}>
                <span style={iconStyle} className={
                    accordionOpen
                        ? "glyphicon glyphicon-triangle-top"
                        : "glyphicon glyphicon-triangle-bottom"
                } />
                {index}. {agenda.title}
            </button>
            {accordionOpen &&
                <div style={contentStyle}>
                    {agenda.caseIDLabel &&
                        <div>
                            <div>
                                {decisionResolutionText} <a href={motionPath}>{openText}</a>
                            </div>
                        </div>
                    }
                    {decision?.caseID &&
                        <div>
                            <div>
                                {decisionText} <a href={decisionPath}>{openText}</a>
                            </div>
                        </div>
                    }
                    {/* 
                    commented out for now as attachments are not returned within agendas yet!!
                     {agenda.attachments?.map((attachment, index) => {
                        return (
                            <div className='attachment' key={'attach' + index}>
                                {t("Attachment")} {index + 1} {''}
                                {attachment.public ?
                                    <a href={attachment.file_uri}>{attachment.name}</a>
                                    : t("Non-public")}
                            </div>
                        )

                    })} */}
                    {agenda.html && (editable ?
                        <EditableItem
                            agendaItem={agenda}
                            meetingId={meetingId}
                            language={"#--LANGUAGE--#"} /> : <div dangerouslySetInnerHTML={{ __html: agenda.html }} />)}

                    {statements && <Statements statements={statements}></Statements>}

                    <div onClick={() => setShowSeatMap(!showSeatMap)}>
                        <span style={iconStyle} className={
                            showSeatMap
                                ? "glyphicon glyphicon-triangle-top"
                                : "glyphicon glyphicon-triangle-bottom"
                        }
                        />
                        <b>{t("Show seat map")}</b>
                    </div>

                    {showSeatMap && <SeatMap meetingId={meetingId} caseNumber={agenda.agendaPoint}></SeatMap>}

                    {voting !== undefined && <Voting voting={voting} meetingId={meetingId} caseNumber={agenda.agendaPoint}></Voting>}
                </div>
            }
        </div>

    )
}

