import { useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { useLanguage } from "../hooks/useLanguage";
import type { Language } from "../contexts/LanguageContext";
import { EmptyState } from "../components/AsyncState";

type Article={category:string;title:string;body:string};
type HelpCopy={eyebrow:string;title:string;intro:string;search:string;empty:string;emptyCopy:string;need:string;needCopy:string;account:string;dashboard:string;articles:Article[]};
const articles:Record<Language,Article[]> = {
  en:[
    {category:"Getting started",title:"Create and configure a project",body:"Open Projects, choose New project, then set its name, language, visibility, and description. Project owners can manage access from Project settings."},
    {category:"Workspace",title:"Work with files and versions",body:"Create nested files and folders in a project workspace. Changes autosave, and version history lets authorized members compare and restore revisions."},
    {category:"Collaboration",title:"Live collaboration and chat",body:"Project members can see presence, remote cursors, typing indicators, workspace channels, and direct conversations."},
    {category:"Planning",title:"Use the Kanban board",body:"Create tasks with priorities, due dates, assignees, and comments. Drag tasks between Todo, Doing, and Done."},
    {category:"AI",title:"Use the AI assistant safely",body:"Explain, fix, optimize, or generate tests from selected code. AI output never overwrites files without confirmation."},
    {category:"Security",title:"Account and access security",body:"JWT authentication and backend authorization protect access. Permissions are verified on every protected request."},
    {category:"Troubleshooting",title:"The API is unavailable",body:"Confirm PostgreSQL is running, then start the API on port 5192. The frontend normally runs on port 5173."},
  ],
  az:[
    {category:"Başlanğıc",title:"Layihə yaradın və tənzimləyin",body:"Layihələr bölməsini açın, Yeni layihə seçin, sonra ad, dil, görünürlük və təsviri daxil edin."},
    {category:"İş sahəsi",title:"Fayllar və versiyalarla işləyin",body:"İç-içə fayl və qovluqlar yaradın. Dəyişikliklər avtomatik saxlanılır, versiyaları müqayisə və bərpa etmək mümkündür."},
    {category:"Əməkdaşlıq",title:"Canlı əməkdaşlıq və söhbət",body:"Üzvlər onlayn istifadəçiləri, uzaq kursorları, yazma göstəricilərini, kanalları və şəxsi söhbətləri görə bilər."},
    {category:"Planlaşdırma",title:"Kanban lövhəsindən istifadə edin",body:"Prioritet, son tarix, icraçı və şərhlərlə tapşırıqlar yaradın. Tapşırıqları sütunlar arasında daşıyın."},
    {category:"Süni intellekt",title:"AI köməkçisindən təhlükəsiz istifadə edin",body:"Seçilmiş kodu izah edin, düzəldin, optimallaşdırın və test yaradın. Təsdiq olmadan fayllar dəyişdirilmir."},
    {category:"Təhlükəsizlik",title:"Hesab və giriş təhlükəsizliyi",body:"JWT autentifikasiyası və backend icazələri girişi qoruyur. Hər qorunan sorğuda icazələr yoxlanılır."},
    {category:"Problemlərin həlli",title:"API əlçatan deyil",body:"PostgreSQL-in işlədiyini yoxlayın, sonra API-ni 5192 portunda başladın. Frontend adətən 5173 portunda işləyir."},
  ],
  ru:[
    {category:"Начало работы",title:"Создание и настройка проекта",body:"Откройте Проекты, выберите Новый проект и укажите название, язык, видимость и описание."},
    {category:"Рабочее пространство",title:"Работа с файлами и версиями",body:"Создавайте вложенные файлы и папки. Изменения сохраняются автоматически, а версии можно сравнивать и восстанавливать."},
    {category:"Совместная работа",title:"Совместная работа и чат",body:"Участники видят присутствие, удалённые курсоры, индикаторы набора, каналы и личные сообщения."},
    {category:"Планирование",title:"Использование Kanban-доски",body:"Создавайте задачи с приоритетами, сроками, исполнителями и комментариями. Перемещайте их между колонками."},
    {category:"ИИ",title:"Безопасная работа с ИИ-помощником",body:"Объясняйте, исправляйте и оптимизируйте выбранный код. Файлы не изменяются без подтверждения."},
    {category:"Безопасность",title:"Безопасность аккаунта и доступа",body:"JWT-аутентификация и серверная авторизация защищают доступ. Права проверяются для каждого запроса."},
    {category:"Устранение неполадок",title:"API недоступен",body:"Убедитесь, что PostgreSQL запущен, затем запустите API на порту 5192. Frontend обычно работает на порту 5173."},
  ],
  de:[
    {category:"Erste Schritte",title:"Projekt erstellen und konfigurieren",body:"Öffnen Sie Projekte, wählen Sie Neues Projekt und legen Sie Name, Sprache, Sichtbarkeit und Beschreibung fest."},
    {category:"Arbeitsbereich",title:"Mit Dateien und Versionen arbeiten",body:"Erstellen Sie verschachtelte Dateien und Ordner. Änderungen werden automatisch gespeichert; Versionen können verglichen und wiederhergestellt werden."},
    {category:"Zusammenarbeit",title:"Live-Zusammenarbeit und Chat",body:"Mitglieder sehen Anwesenheit, entfernte Cursor, Tippanzeigen, Kanäle und Direktnachrichten."},
    {category:"Planung",title:"Kanban-Board verwenden",body:"Erstellen Sie Aufgaben mit Prioritäten, Fristen, Zuständigen und Kommentaren und verschieben Sie sie zwischen Spalten."},
    {category:"KI",title:"KI-Assistent sicher verwenden",body:"Erklären, korrigieren oder optimieren Sie ausgewählten Code. Dateien werden nie ohne Bestätigung geändert."},
    {category:"Sicherheit",title:"Konto- und Zugriffssicherheit",body:"JWT-Authentifizierung und serverseitige Autorisierung schützen den Zugriff. Berechtigungen werden bei jeder Anfrage geprüft."},
    {category:"Fehlerbehebung",title:"Die API ist nicht verfügbar",body:"Prüfen Sie PostgreSQL und starten Sie die API auf Port 5192. Das Frontend läuft normalerweise auf Port 5173."},
  ],
  tr:[
    {category:"Başlarken",title:"Proje oluşturun ve yapılandırın",body:"Projeler'i açın, Yeni proje'yi seçin; ad, dil, görünürlük ve açıklamayı belirleyin."},
    {category:"Çalışma alanı",title:"Dosyalar ve sürümlerle çalışın",body:"İç içe dosya ve klasörler oluşturun. Değişiklikler otomatik kaydedilir; sürümler karşılaştırılıp geri yüklenebilir."},
    {category:"İş birliği",title:"Canlı iş birliği ve sohbet",body:"Üyeler çevrimiçi kullanıcıları, uzak imleçleri, yazma göstergelerini, kanalları ve özel konuşmaları görebilir."},
    {category:"Planlama",title:"Kanban panosunu kullanın",body:"Öncelik, son tarih, atanan kişiler ve yorumlarla görevler oluşturup sütunlar arasında taşıyın."},
    {category:"Yapay zekâ",title:"AI asistanını güvenle kullanın",body:"Seçili kodu açıklayın, düzeltin veya optimize edin. Onay olmadan dosyalar değiştirilmez."},
    {category:"Güvenlik",title:"Hesap ve erişim güvenliği",body:"JWT kimlik doğrulaması ve backend yetkilendirmesi erişimi korur. Her korumalı istekte izinler denetlenir."},
    {category:"Sorun giderme",title:"API kullanılamıyor",body:"PostgreSQL'in çalıştığını doğrulayın ve API'yi 5192 portunda başlatın. Frontend normalde 5173 portunda çalışır."},
  ],
};
const copy:Record<Language,Omit<HelpCopy,"articles">>={
 en:{eyebrow:"HELP CENTER",title:"What can we help you build?",intro:"Search product guidance, collaboration workflows, and troubleshooting steps.",search:"Search help articles…",empty:"No matching articles",emptyCopy:"Try broader words such as project, files, collaboration, or security.",need:"Still need help?",needCopy:"Review project settings or return to the dashboard to check service status.",account:"Account settings",dashboard:"Dashboard"},
 az:{eyebrow:"YARDIM MƏRKƏZİ",title:"Nə yaratmağınıza kömək edə bilərik?",intro:"Məhsul təlimatları, əməkdaşlıq axınları və problemlərin həlli üzrə axtarın.",search:"Yardım məqalələrində axtar…",empty:"Uyğun məqalə tapılmadı",emptyCopy:"Layihə, fayl, əməkdaşlıq və ya təhlükəsizlik kimi daha ümumi sözlər sınayın.",need:"Hələ də kömək lazımdır?",needCopy:"Layihə parametrlərini yoxlayın və ya xidmət vəziyyəti üçün idarəetmə panelinə qayıdın.",account:"Hesab parametrləri",dashboard:"İdarəetmə paneli"},
 ru:{eyebrow:"ЦЕНТР ПОМОЩИ",title:"Что мы поможем вам создать?",intro:"Ищите руководства, процессы совместной работы и способы устранения неполадок.",search:"Поиск по справочным статьям…",empty:"Статьи не найдены",emptyCopy:"Попробуйте более общие слова: проект, файлы, совместная работа или безопасность.",need:"Нужна дополнительная помощь?",needCopy:"Проверьте настройки проекта или вернитесь на панель для просмотра состояния сервисов.",account:"Настройки аккаунта",dashboard:"Панель управления"},
 de:{eyebrow:"HILFEZENTRUM",title:"Was möchten Sie erstellen?",intro:"Durchsuchen Sie Anleitungen, Arbeitsabläufe und Schritte zur Fehlerbehebung.",search:"Hilfeartikel durchsuchen…",empty:"Keine passenden Artikel",emptyCopy:"Versuchen Sie allgemeinere Begriffe wie Projekt, Dateien, Zusammenarbeit oder Sicherheit.",need:"Benötigen Sie weitere Hilfe?",needCopy:"Prüfen Sie die Projekteinstellungen oder den Dienststatus im Dashboard.",account:"Kontoeinstellungen",dashboard:"Dashboard"},
 tr:{eyebrow:"YARDIM MERKEZİ",title:"Ne oluşturmanıza yardımcı olabiliriz?",intro:"Ürün rehberleri, iş birliği akışları ve sorun giderme adımlarında arama yapın.",search:"Yardım makalelerinde ara…",empty:"Eşleşen makale yok",emptyCopy:"Proje, dosya, iş birliği veya güvenlik gibi daha genel kelimeler deneyin.",need:"Hâlâ yardıma mı ihtiyacınız var?",needCopy:"Proje ayarlarını inceleyin veya hizmet durumunu kontrol etmek için panele dönün.",account:"Hesap ayarları",dashboard:"Kontrol paneli"},
};

export function HelpCenterPage(){
 const{language}=useLanguage();const text={...copy[language],articles:articles[language]};const[params]=useSearchParams();const[search,setSearch]=useState(params.get("topic")??"");const[open,setOpen]=useState<string>();
 const results=useMemo(()=>text.articles.filter(item=>`${item.category} ${item.title} ${item.body}`.toLocaleLowerCase(language).includes(search.toLocaleLowerCase(language))),[language,search,text.articles]);
 return <main className="dashboard-content help-page"><header className="help-hero"><p className="dashboard-date">{text.eyebrow}</p><h1>{text.title}</h1><p>{text.intro}</p><label><span aria-hidden="true">⌕</span><input autoFocus type="search" value={search} onChange={e=>setSearch(e.target.value)} placeholder={text.search}/></label></header><section className="help-grid">{results.map(article=><article key={article.title} className={open===article.title?"open":""}><button aria-expanded={open===article.title} onClick={()=>setOpen(open===article.title?undefined:article.title)}><span><small>{article.category}</small><b>{article.title}</b></span><i aria-hidden="true">{open===article.title?"−":"+"}</i></button>{open===article.title&&<p>{article.body}</p>}</article>)}</section>{!results.length&&<EmptyState title={text.empty} description={text.emptyCopy}/>}<section className="help-contact"><div><h2>{text.need}</h2><p>{text.needCopy}</p></div><Link className="ui-button ghost" to="/settings">{text.account}</Link><Link className="ui-button primary" to="/dashboard">{text.dashboard}</Link></section></main>;
}
