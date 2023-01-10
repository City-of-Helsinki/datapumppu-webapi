import React, { useEffect } from 'react'
import AgendaItem from './AgendaItem'
import Login from './Login';
import Header from './Header';

export default function Meeting() {
    const [agenda, setAgenda] = React.useState([])

    const [showHeader, setShowHeader] = React.useState(false)
    const [showLogin, setShowLogin] = React.useState(false)
    const [loggedIn, setLoggedIn] = React.useState(false)

    const style = {
        agenda: {
            paddingTop: "35px"
        }
    }

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
                    }
                })
        }
        fetchData()
    }, [setAgenda])

    window.onkeydown = event => {
        switch (event.keyCode) {
            case 120: //F9
                if (loggedIn) setShowHeader(!showHeader)
                else setShowLogin(!showLogin)
                break
        }
    }

    const submitLogin = async (username, password) => {
        const request = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username: username, password: password })
        }
        var response = await fetch('#--API_URL--#/login/', request)
        var body = await response.json()
        if (response.ok) {
            localStorage.setItem("userToken", body.token)
            setLoggedIn(true)
            setShowHeader(true)
            setShowLogin(false)
        } else {
            console.log("login failed")
        }
    }

    const submitLogout = async () => {
        const request = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem("userToken")
            }
        }
        var response = await fetch('#--API_URL--#/logout', request)
        if (response.ok) {
            localStorage.clear()
            setLoggedIn(false)
            setShowHeader(false)
        } else {
            console.log("logout failed")
        }
    }

    return (
        <div className="container">
            {showHeader && <Header submitLogout={submitLogout}/>}
            {showLogin && <Login submitLogin={submitLogin} closeLogin={() => setShowLogin(false)} />}
            <div style={style.agenda}>
                {agenda?.sort((a, b) => (a.agendaPoint - b.agendaPoint)).map((agendaItem, index) => {
                    return <AgendaItem key={index} index={index + 1} agenda={agendaItem} />
                })}
            </div>
        </div>
    )
}
