
import React, { useEffect } from 'react'
import AgendaItem from './AgendaItem'

export default function Meeting() {
    const [agenda, setAgenda] = React.useState([]);
    useEffect(() => {
        const fetchData = async () => {
            await fetch('#--API_URL--#/meetings/meeting?year=#--MEETING_YEAR--#&sequenceNumber=#--MEETING_SEQUENCE_NUM--#')
                .then(async (res) => {
                    return await res.json()
                })
                .then((json) => {
                    console.log(json)
                    setAgenda(json.agendas)
                })
        }
        fetchData()
    }, [setAgenda])

    return (
        <div className="container">
            {agenda?.map((agendaItem, index) => {
                return <AgendaItem key={index} index={index + 1} agenda={agendaItem} />
            })}
        </div>
    )
}





