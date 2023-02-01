import { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next';
import SeatMap from './SeatMap'
import Voting from './Voting'
import Statements from './Statements'
import {
    itemStyle,
    itemOpenStyle,
    agendaButtonStyle,
    titleStyle,
    contentStyle,
    attachmentTable,
    linkStyle
} from './styles';
import EditableItem from './EditableItem';
import { FaCaretUp, FaCaretDown } from "react-icons/fa";

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

    var motionPath = `https://paatokset.hel.fi/#--LANGUAGE--#/asia/${agenda?.caseIDLabel?.replace(" ", "-")}#`
    var decisionPath = `https://paatokset.hel.fi/#--LANGUAGE--#/asia/${decision?.caseID}?paatos=${decision?.nativeId.replace("/[{}]/g", "")}`
    
    if (parseInt("#--MEETING_YEAR--#") < 2018 ||(parseInt("#--MEETING_YEAR--#") == 2018 && parseInt("#--MEETING_SEQUENCE_NUM--#") < 4)) {
        motionPath = "https://dev.hel.fi/paatokset/asia/" + agenda?.caseIDLabel?.replace(" ", "-").toLowerCase() + "/kvsto-#--MEETING_YEAR--#-#--MEETING_SEQUENCE_NUM--#"
        decisionPath = "https://dev.hel.fi/paatokset/asia/" + decision?.caseID
    }

    return (
        <div style={accordionOpen ? itemOpenStyle : itemStyle}>
            <div style={titleStyle}>
                <button style={agendaButtonStyle} onClick={() => setAccordionOpen(!accordionOpen)}>
                    <div style={{ paddingRight: "10px", marginTop: "4px" }}>
                        {accordionOpen
                            ? <FaCaretUp />
                            : <FaCaretDown />}
                    </div>
                    <div style={{ paddingRight: "10px" }}>
                        {index}.</div>
                    <div style={{ paddingRight: "10px" }}>
                        {agenda.title}</div>
                </button>
                {accordionOpen && <a style={agendaButtonStyle} href={`#T${agenda.videoPosition}`}>{t('Go to video position')}</a>}
            </div>
            {accordionOpen &&
                <div style={contentStyle}>
                    <div style={attachmentTable.table}>
                        {
                            agenda.caseIDLabel &&
                            <div style={attachmentTable.row}>
                                <div style={attachmentTable.cell}>
                                    {decisionResolutionText}
                                </div>
                                <div style={attachmentTable.cell}>
                                    <a style={linkStyle} href={motionPath} target="_blank">{openText}</a>
                                </div>
                            </div>
                        }
                        {decision?.caseID &&
                            <div style={attachmentTable.row}>
                                <div style={attachmentTable.cell}>
                                    {decisionText}
                                </div>
                                <div style={attachmentTable.cell}>
                                    <a style={linkStyle} href={decisionPath} target="_blank">{openText}</a>
                                </div>
                            </div>
                        }
                        {agenda.attachments?.sort((a, b) => (a.attachmentNumber - b.attachmentNumber)).map((attachment, index) => {
                            return (
                                <div className='attachment' key={'attach' + index} style={attachmentTable.row}>
                                    <div style={attachmentTable.cell}>
                                        {t("Attachment")} {attachment.attachmentNumber} {''}
                                    </div>
                                    <div style={attachmentTable.cell}>

                                        {attachment.fileURI ?
                                            <a style={linkStyle} href={attachment.fileURI}>{attachment.title}</a>
                                            : t("Non-public")}
                                    </div>
                                </div>
                            )
                        })
                        }
                    </div>
                    {agenda.html && (editable ?
                        <EditableItem
                            agendaItem={agenda}
                            meetingId={meetingId}
                            language={"#--LANGUAGE--#"} /> 
                            : 
                            <div dangerouslySetInnerHTML={{ __html: agenda.html }} />)}
                    {statements && <Statements statements={statements}></Statements>}
                    <div style={{ padding: "30px 10px 0 0" }}>
                        <button style={agendaButtonStyle} onClick={() => setShowSeatMap(!showSeatMap)}>
                            <div style={{ paddingRight: "10px", marginTop: "4px" }}>
                                {showSeatMap
                                    ? <FaCaretUp />
                                    : <FaCaretDown />}
                            </div>
                            <b>{t("Show seat map")}</b>
                        </button>
                    </div>

                    {showSeatMap && <SeatMap meetingId={meetingId} caseNumber={agenda.agendaPoint}></SeatMap>}

                    {voting !== undefined && <Voting voting={voting} meetingId={meetingId} caseNumber={agenda.agendaPoint}></Voting>}
                </div>
            }
        </div>

    )
}

