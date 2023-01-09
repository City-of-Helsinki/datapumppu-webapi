
import { useEffect, useState } from 'react'
import AgendaItem from './AgendaItem'

const containerStyle = {
    maxWidth: "800px",
    WebkitTransition: "all 2s ease",
    MozTransition: "all 2s ease",
    OTransition: "all 2s ease",
    transition: "all 2s ease"  
}

export default function Meeting() {
    const [agenda, setAgenda] = useState([]);
    const [decisions, setDecisions] = useState([]);
    const [meetingId, setMeetingId] = useState("");
    useEffect(() => {
        const fetchData = async () => {
            await fetch('#--API_URL--#/meetings/meeting?year=#--MEETING_YEAR--#&sequenceNumber=#--MEETING_SEQUENCE_NUM--#&lang=#--LANGUAGE--#')
                .then(async (res) => {
                    if (res.status !== 204) {
                        return await res.json()
                    }
                })
                .then((json) => {
                    if (json && Object.keys(json).length > 0) {
                        setAgenda(json.agendas)
                        setDecisions(json.decisions)
                        setMeetingId(json.meetingID)
                    }
                })
        }
        fetchData()
    }, [setAgenda])

    return (
        <div style={containerStyle}>
            {agenda?.sort((a, b) => (a.agendaPoint - b.agendaPoint)).map((agendaItem, index) => {
                return <AgendaItem
                    key={index}
                    index={index + 1}
                    agenda={agendaItem}
                    decision={decisions?.find(d => d.caseIDLabel === agendaItem.caseIDLabel)}
                    meetingId={meetingId}
                />
            })}
        </div>
    )
}





