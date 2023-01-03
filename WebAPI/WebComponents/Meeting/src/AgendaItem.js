import { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next';
import SeatMap from './SeatMap'
import Voting from './Voting'

export default function AgendaItem(props) {
    const [accordionOpen, setAccordionOpen] = useState(false)
    const [showSeatMap, setShowSeatMap] = useState(false)
    const [voting, setVoting] = useState(undefined)
    const { agenda, index, meetingId } = props
    const { t } = useTranslation();
    
    useEffect(() => {
        const fetchData = async () => {
            const response = await fetch(`#--API_URL--#/voting/${meetingId}/${agenda.agendaPoint}`)
            if (response.status === 200) {
                const votingData = await response.json()
                setVoting(votingData)
            }
        }

        if (accordionOpen === true && voting === undefined) {
            fetchData()
        }
    }, [accordionOpen])

    const decisionResolutionText = t('Decision resolution')
    const decisionText = t('Decision')
    const openText = t('Open')
    const motionPath = `#--API_URL--#/components/pages/motion.html?caseIdLabel=${agenda.caseIDLabel}&lang=fi`
    const decisionPath = `#--API_URL--#/components/pages/decision.html?caseIdLabel=${agenda.caseIDLabel}&lang=fi`
    return (
        <div className={accordionOpen ? "item item-open" : 'item'}>
            <button className='agendaTitleButton' onClick={() => setAccordionOpen(!accordionOpen)}>
                <span className={
                    accordionOpen
                        ? "icon glyphicon glyphicon-triangle-top"
                        : "icon glyphicon glyphicon-triangle-bottom"
                } />
                {index}. {agenda.title}
            </button>
            {accordionOpen &&
                <div className='content'>
                    {agenda.caseIDLabel &&
                    <div>
                        <div>
                            {decisionResolutionText} <a href={motionPath}>{openText}</a>
                        </div>
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
                    {agenda.html && <div dangerouslySetInnerHTML={{__html: agenda.html}} />}

                    <div onClick={() => setShowSeatMap(!showSeatMap)}>
                        <span className={
                            showSeatMap
                                ? "icon glyphicon glyphicon-triangle-top"
                                : "icon glyphicon glyphicon-triangle-bottom"
                        } 
                        />
                        <bold>{t("Show seat map")}</bold>
                    </div>

                    {showSeatMap && <SeatMap meetingId={meetingId} caseNumber={agenda.agendaPoint}></SeatMap>}

                    {voting !== undefined && <Voting voting={voting} meetingId={meetingId} caseNumber={agenda.agendaPoint}></Voting>}
                </div>
            }
        </div>

    )
}

