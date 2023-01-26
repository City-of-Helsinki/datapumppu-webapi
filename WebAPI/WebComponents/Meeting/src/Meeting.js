
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import AgendaItem from './AgendaItem';
import Login from './Login';
import Header from './Header';
import SyncBar from './SyncBar';
import {
    containerStyle,
    agendaStyle,
} from './styles';

const agendaTitleStyle = {
    backgroundColor: "inherit",
    border: "none",
    fontWeight: "bold",
    textAlign: "start",
    padding: "8px 8px 6px 8px",
    color: "black"
}

export default function Meeting() {
    const [accordionOpen, setAccordionOpen] = useState(true);
    const [agenda, setAgenda] = useState([]);
    const [decisions, setDecisions] = useState([]);
    const [meetingId, setMeetingId] = useState("");

    const [showHeader, setShowHeader] = useState(false)
    const [showLogin, setShowLogin] = useState(false)
    const [showSyncBar, setShowSyncBar] = useState(false)
    const [loggedIn, setLoggedIn] = useState(false)

    const { t } = useTranslation();

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
        var response = await fetch('#--API_URL--#/editor/login/', request)
        if (response.ok) {
            var body = await response.json()
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
        var response = await fetch('#--API_URL--#/editor/logout', request)
        if (response.ok) {
            localStorage.clear()
            setLoggedIn(false)
            setShowHeader(false)
        } else {
            console.log("logout failed")
        }
    }

    return (
        <div style={containerStyle}>
            {showHeader && <Header submitLogout={submitLogout} toggleSyncBar={() => setShowSyncBar(!showSyncBar)} />}
            {showLogin && <Login submitLogin={submitLogin} closeLogin={() => setShowLogin(false)} />}
            <div style={agendaTitleStyle} onClick={() => setAccordionOpen(!accordionOpen)}>
                {accordionOpen && <span className='glyphicon glyphicon-chevron-down'></span>}
                {t('Agenda and Proceeding').toUpperCase()}
            </div>
            {accordionOpen &&
                <div style={agendaStyle}>
                    {agenda?.sort((a, b) => (a.agendaPoint - b.agendaPoint)).map((agendaItem, index) => {
                        return <AgendaItem
                            editable={loggedIn}
                            key={index}
                            index={index + 1}
                            agenda={agendaItem}
                            decision={decisions?.find(d => d.caseIDLabel === agendaItem.caseIDLabel)}
                            meetingId={meetingId}
                        />
                    })}
                </div>
            }
            {showSyncBar && agenda &&
                <SyncBar
                    meetingId={meetingId}
                    agendaPointTimestamp={agenda.find(item => item.agendaPoint === 2)?.timestamp}
                    closeSyncBar={() => setShowSyncBar(false)}
                />
            }
        </div>
    )
}
