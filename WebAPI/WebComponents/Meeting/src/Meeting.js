
import { useEffect, useState } from 'react'
import AgendaItem from './AgendaItem'

export default function Meeting() {
    const [agenda, setAgenda] = useState([]);
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
                        setMeetingId(json.meetingID)
                    }
                })
        }
        fetchData()
    }, [setAgenda])

    return (
        <div className="container">
            {agenda?.sort((a, b) => (a.agendaPoint - b.agendaPoint)).map((agendaItem, index) => {
                return <AgendaItem key={index} index={index + 1} agenda={agendaItem} meetingId={meetingId}/>
            })}
        </div>
    )
}





