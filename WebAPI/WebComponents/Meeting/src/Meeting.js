import { useEffect, useState } from 'react'
import { useTranslation, I18nextProvider } from 'react-i18next'
import i18n from './i18n';
import AgendaItem from './AgendaItem'
import Login from './Login'
import Header from './Header'
import SyncBar from './SyncBar'
import {
    containerStyle,
    agendaButtonStyle
} from './styles';
import { FaCaretDown, FaCaretUp } from "react-icons/fa";
import {
    HubConnectionBuilder,
    LogLevel
} from '@microsoft/signalr'

export default function Meeting() {
    const [accordionOpen, setAccordionOpen] = useState(true)
    const [agenda, setAgenda] = useState([])
    const [decisions, setDecisions] = useState([])
    const [updated, setUpdated] = useState(0)
    const [updatedCaseNumber, setUpdatedCaseNumber] = useState("")
    const [meetingId, setMeetingId] = useState("")

    const [showHeader, setShowHeader] = useState(false)
    const [showLogin, setShowLogin] = useState(false)
    const [showSyncBar, setShowSyncBar] = useState(false)
    const [loggedIn, setLoggedIn] = useState(false)
    const [isLiveMeeting, setIsLiveMeeting] = useState(false)

    const { t } = useTranslation()
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
                    setIsLiveMeeting(json.meetingEnded === null)
                }
            })
    }

    useEffect(() => {
        fetchData()
    }, [])

    useEffect(() => {
        if (meetingId?.length > 0) {
            const connection = new HubConnectionBuilder()
                .withUrl("#--API_URL--#/live")
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                .build();

            connection.on("receiveMessage", (message) => {
                if (message.meetingId == meetingId) {
                    setUpdatedCaseNumber(message.caseNumber)
                    setUpdated((prevVal) => prevVal + 1)
                }
            })

            connection.start()
                .then(() => console.log("Connected"))
                .catch(err => console.log(err))
        }
    }, [meetingId])

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
        if (!response.ok) {
            console.log("logout failed")
        }
        localStorage.clear()
        setLoggedIn(false)
        setShowHeader(false)
    }

    return (
        <I18nextProvider i18n={i18n}>
            <div style={containerStyle}>
                {showHeader && <Header submitLogout={submitLogout} toggleSyncBar={() => setShowSyncBar(!showSyncBar)} />}
                {showLogin && <Login submitLogin={submitLogin} closeLogin={() => setShowLogin(false)} />}
                <button style={agendaButtonStyle} onClick={() => setAccordionOpen(!accordionOpen)}>
                    <div style={{ paddingRight: "10px", marginTop: "4px" }}>
                        {accordionOpen ? <FaCaretUp /> : <FaCaretDown />}
                    </div>
                    {t('Agenda and Proceeding').toUpperCase()}
                </button>
                {accordionOpen &&
                    agenda?.sort((a, b) => (a.agendaPoint - b.agendaPoint)).map((agendaItem, index) => {
                        return <AgendaItem
                            updated={updated}
                            updatedCaseNumber={updatedCaseNumber}
                            editable={loggedIn}
                            key={index}
                            index={index + 1}
                            agenda={agendaItem}
                            decision={decisions?.find(d => d.caseIDLabel === agendaItem.caseIDLabel)}
                            meetingId={meetingId}
                            isLiveMeeting={isLiveMeeting}
                        />
                    })
                }
                {showSyncBar && agenda &&
                    <SyncBar
                        meetingId={meetingId}
                        agendaPointTimestamp={agenda.find(item => item.agendaPoint === 2)?.timestamp}
                        closeSyncBar={() => setShowSyncBar(false)}
                    />
                }
            </div>
        </I18nextProvider>
    )
}
