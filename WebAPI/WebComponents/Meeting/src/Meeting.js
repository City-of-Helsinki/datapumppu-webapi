
import React, { useEffect } from 'react'
import AgendaItem from './AgendaItem'
export default function Meeting() {
  const [agenda, setAgenda] = React.useState([]);

  useEffect(() => {
    const fetchData = async () => {
      const response = await fetch('https://dev.hel.fi:443/paatokset/v1/agenda_item/?meeting=69977') // old api
      const json = await response.json()
      setAgenda(json.objects)
    }

    fetchData()
  }, [setAgenda])

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("#--API_URL--#/live")
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.start().then(result => {

      console.log("Connected");
      connection.on("ReceiveMessage", message => {
        console.log("Message received; ", message)
      })


    }).catch(err => console.log(err))
  },  [])


  return (
    <div className="container">
      {agenda.map((agendaItem, index) => {
        return <AgendaItem key={index} index={index + 1} agenda={agendaItem} />
      })}
    </div>
  )
}


