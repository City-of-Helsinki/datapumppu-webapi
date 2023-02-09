import { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next'

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
    linkStyle,
    headingStyle
} from './styles';
import EditableItem from './EditableItem';
import { FaCaretUp, FaCaretDown } from "react-icons/fa";

export default function AgendaItem(props) {
    const [accordionOpen, setAccordionOpen] = useState(false)
    const [showSeatMap, setShowSeatMap] = useState(false)
    const [voting, setVoting] = useState(undefined)
    const [statements, setStatements] = useState(undefined)
    const [reservations, setReservations] = useState(undefined)
    const [editableHTML, setEditableHTML] = useState(null)
    const { agenda, index, meetingId, decision, editable, updated, updatedCaseNumber } = props
    const { t } = useTranslation();

    useEffect(() => {
        if (agenda.agendaPoint == updatedCaseNumber && accordionOpen) {
            fetchStatementsData()
            fetchVotingData()
            fetchReservationsData()
        }
    }, [updated, updatedCaseNumber])

    useEffect(() => {
        if (accordionOpen === true) {
            fetchStatementsData()
            fetchVotingData()
            fetchReservationsData()
        }
    }, [accordionOpen])

    useEffect(() => {
        setEditableHTML(manageContent(agenda.html))
    }, [])

    const fetchReservationsData = async () => {
        const response = await fetch(`#--API_URL--#/reservations/${meetingId}/${agenda.agendaPoint}`)
        if (response.status === 200) {
            const data = await response.json()
            setReservations(data)
        }
    }

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

    const manageContent = (html) => {
        var div = document.createElement('div')
        var newDiv = document.createElement('div')
        div.innerHTML = html
        const section = div.querySelector(".SisaltoSektio") || div
        const nodes = section.childNodes
        for (var i = 0; i < nodes.length; i++) {
            var element = nodes[i].cloneNode(true)
            if (element.nodeName != 'H1' && element.nodeName != 'H2' && element.nodeName != 'H3' && element.nodeName != 'H4') {
                if (element.nodeType == 3) {
                    var p = document.createElement('p')
                    p.textContent = element.textContent
                    element = p
                }
                element.style.fontSize = "16px"
                element.style.fontFamily = "Verdana, Arial, sans-serif"
                element.style.color = "#414143"
                element.style.lineHeight = "1.4"
                newDiv.appendChild(element)
            }
        }
        return newDiv.innerHTML
    }

    const decisionResolutionText = t('Decision resolution')
    const decisionText = t('Decision')
    const openText = t('Open')

    var motionPath = `https://paatokset.hel.fi/#--LANGUAGE--#/asia/${agenda?.caseIDLabel?.replace(" ", "-")}#`
    var decisionPath = `https://paatokset.hel.fi/#--LANGUAGE--#/asia/${decision?.caseID}?paatos=${decision?.nativeId.replace("/[{}]/g", "")}`

    if (parseInt("#--MEETING_YEAR--#") < 2018 || (parseInt("#--MEETING_YEAR--#") == 2018 && parseInt("#--MEETING_SEQUENCE_NUM--#") < 4)) {
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
                        {agenda.caseIDLabel &&
                            <div style={attachmentTable.row}>
                                <div style={attachmentTable.label}>
                                    {decisionResolutionText}
                                </div>
                                <div style={attachmentTable.cell}>
                                    <a style={linkStyle} href={motionPath} target="_blank">{openText}</a>
                                </div>
                            </div>
                        }
                        {decision?.caseID &&
                            <div style={attachmentTable.row}>
                                <div style={attachmentTable.label}>
                                    {decisionText}
                                </div>
                                <div style={attachmentTable.cell}>
                                    <a style={linkStyle} href={decisionPath} target="_blank">{openText}</a>
                                </div>
                            </div>
                        }
                    </div>
                    <div style={{ padding: "20px 0px 20px 0px" }}>
                        <div style={headingStyle}>{decisionText}</div>
                        {agenda.html && (editable ?
                            <EditableItem
                                agendaItem={agenda}
                                editableHTML={editableHTML}
                                meetingId={meetingId}
                                language={"#--LANGUAGE--#"} />
                            :
                            <div dangerouslySetInnerHTML={{ __html: editableHTML }} />
                        )}             
                    </div>
                    <div style={attachmentTable.table}>
                        {agenda.attachments?.sort((a, b) => (a.attachmentNumber - b.attachmentNumber)).map((attachment, index) => {
                            return (
                                <div className='attachment' key={'attach' + index} style={attachmentTable.row}>
                                    <div style={attachmentTable.label}>
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

                    {(statements || reservations) && <Statements statements={statements} reservations={reservations}></Statements>}
                    <div style={{ padding: "30px 10px 0 0" }}>
                        <button style={agendaButtonStyle} onClick={() => setShowSeatMap(!showSeatMap)}>
                            <div style={{ paddingRight: "10px", marginTop: "4px" }}>
                                {showSeatMap
                                    ? <FaCaretUp />
                                    : <FaCaretDown />}
                            </div>
                            {t("Show seat map")}
                        </button>
                    </div>

                    {showSeatMap && <SeatMap
                        meetingId={meetingId}
                        caseNumber={agenda.agendaPoint}
                        updated={updated}
                        updatedCaseNumber={updatedCaseNumber}
                    >
                    </SeatMap>
                    }

                    {voting &&
                        voting.map((vote, index) => (
                            <Voting
                                voting={vote}
                                meetingId={meetingId}
                                caseNumber={agenda.agendaPoint}
                                index={index}
                                updated={updated}
                                updatedCaseNumber={updatedCaseNumber}
                                title={agenda.title}>
                            </Voting>
                        ))
                    }
                </div>
            }
        </div>

    )
}

