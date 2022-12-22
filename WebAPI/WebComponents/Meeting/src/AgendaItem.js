
import React, { useState } from 'react'
import ReactDOM from 'react-dom'
import EditableItem from './EditableItem';
import { useTranslation } from 'react-i18next';
import './theme.css'

export default function AgendaItem(props) {
    const [accordionOpen, setAccordionOpen] = React.useState(false)
    const agenda = props.agenda
    const index = props.index
    const { t } = useTranslation();
    
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
                <div className='content' >
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
                </div>
            }
        </div>

    )
}

