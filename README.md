# PamelloV7

A **modern, functional, easy to use** Music Bot

> _Note: This README is currently in an unfinished state until the release date on 27.07.2026_

> *Note: PamelloV7 is still in very active development, but is already in a **good enough state** to use. Please join the [PamelloV7 Development]() Discord server to give your feedback, suggestions, or report your problem. You can also always just [DM me]()*

## Features

- [Simple setup](#setup), **no Lavalink**, just paste your token into a config
- [Interactive & updatable Discord messages](#discord) (using **ComponentsV2**)
- [Built first for Discord, but not around Discord](). PamelloV7 is **independent of Discord, and more platforms will be added** over the course of development
- [Multi Authorizations](#database) per Pamello user, preserving your favorites and everything else between multiple Discord accounts
- [Player & Speaker separation](#playback) allowing for Multi Guild / Voice Channel / Platform playback of the same/different queues
- [Lightweight to run](#playback), about **150 MB of RAM** usage on average
- [Fast playback](#playback) with song **caching / chunking**
- [Browse all songs](#database), playlists, and favorites of **any user**
- [PEQL](#peql) to query multiple songs or other entities with a single query
- [Play To Radio](#radio) to listen in games like Euro Truck Simulator 2, or **any other app** that supports internet radio *(HTTP MP3 audio stream)*
- Supports multiple music platforms: **YouTube**, **SoundCloud**, **osu!**, and more like Spotify and others in the future

## Planned Features

- [Client App](#client-app) to control playback, browse songs and **listen to music directly**, even without a platform like Discord
- [Launcher App](#launcher-app) **to automate installation, configuration, and updates** of the Server *(bot)*, Client, and Modules
- [Extensions](#modules) using modules or scripts *(aka plugins, mods)*
- [Integrations](#api) using SignalR / REST API 
- [Audio Routing System](#interactive-audio) that can be **viewed and edited** in live mode
- [Actions History](#history) with a **yearly rewind** like in Spotify and other apps

> *Planned Features Note: Some planned features are already present, specifically **history, modules, and API**. But they are not intended for a real user for now, they are more just for me during the development time. But if you are also a developer and really want to, you can use them too. Just be ready for a lot of breaking changes when those features are actually introduced publicly. Ah and also there is no documentation yet*

---

## Setup

<details>
<summary><h4>How to create your Discord bot account</h4></summary>

Go to https://discord.com/developers/applications, log into your account, and click "New Application", you can give it any name you want

<img width="1589" height="341" alt="image" src="https://github.com/user-attachments/assets/087ea5f8-c760-4c94-be7c-cd81401a5b26" />

After creating an application, go into the "Bot" tab, and enable the following intents:
- Presence Intent
- Server Members Intent
- Message Content Intent

> *Note: Pamello doesn't use most of its intents right now, but you have to activate them because it expects them*

<img width="1607" height="342" alt="image" src="https://github.com/user-attachments/assets/2cf8ab07-2875-4530-949b-6cf569fb2c58" />

Now, to add it to your server, go to the "OAuth2" tab, and check the boxes as on the screenshot

<img width="1596" height="627" alt="image" src="https://github.com/user-attachments/assets/941cb505-266c-4cb5-beeb-0f9293d22ab1" />

Scroll to the bottom, and copy the "Generated URL", open it in a new tab, and select your server

<img width="1330" height="113" alt="image" src="https://github.com/user-attachments/assets/4a2393e2-35f5-4be9-b085-905f3b3e84cd" />

Lastly, go back into the "Bot" tab, reset your token, and copy it

You can also change your bot Icon, Banner, and Username here if you want

<img width="1593" height="731" alt="image" src="https://github.com/user-attachments/assets/807df08d-bbd5-4cdc-810a-cd296d530751" />

This is the token you need to paste into a config. If you haven't installed the bot yet, read the installation section next

</details>

<details>
<summary><h4>How to install PamelloV7</h4></summary>

Go to the [download](#download) section and download the release for your system (this tutorial is for Windows, but it's pretty much the same on other systems too)

After the .zip archive is downloaded, extract it into any directory you'd like

<img width="667" height="277" alt="image_2026-07-25_10-26-29" src="https://github.com/user-attachments/assets/4afd53a3-01ac-4289-bc10-964173bc219a" />

Go to the extracted directory, locate the `start.bat` (or `start.sh`) file, and run it

<img width="615" height="342" alt="image_2026-07-25_10-29-00" src="https://github.com/user-attachments/assets/64cf66e4-b6b1-4a49-aa2c-dcbd87c22907" />

The console will open, with Pamello saying that it created a config example and where it is, copy that file path 

<img width="1048" height="115" alt="image_2026-07-25_10-32-53" src="https://github.com/user-attachments/assets/6a6f7b89-70dc-4251-969f-d958449ac082" />

Paste that path into the path input at the top of your explorer, and press Enter

<img width="1002" height="182" alt="image_2026-07-25_10-39-42" src="https://github.com/user-attachments/assets/19f9b8ec-4917-4a2c-814b-e0d92627632f" />

This will either open the file in your text editor, or ask you to select an app to open it with, select any text editor you want

For now you can basically just replace `YOUR DISCORD BOT TOKEN HERE` with your Discord bot token, but still **don't forget to read the IMPORTANT section** at the top for the full instructions!

<img width="1092" height="599" alt="image_2026-07-25_10-43-31" src="https://github.com/user-attachments/assets/c73bc832-760e-4161-8afc-c3c31daf5c03" />

Your token should look like this

<img width="737" height="134" alt="image_2026-07-25_10-55-08" src="https://github.com/user-attachments/assets/6870dfa7-892c-49e6-b740-163037479960" />

After you are done with the edits, be sure to save the file, and close it

(Though you can read some more if you're curious what else you can configure there)

<img width="263" height="286" alt="image_2026-07-25_10-48-20" src="https://github.com/user-attachments/assets/583acca1-b1b7-4eb9-ab1c-0d291f4fba59" />

And lastly, rename your `configEXAMPLE.jsonc` to `config.jsonc`

<img width="444" height="96" alt="image_2026-07-25_10-49-04" src="https://github.com/user-attachments/assets/d4e01886-fd22-4e08-ae3a-1a5d082427e3" />

This is how it should look

<img width="400" height="58" alt="image" src="https://github.com/user-attachments/assets/d01db416-7db2-447d-8399-1f79a5742534" />

That's it, now you can start `start.bat` once more, this time Pamello should start normally

Optionally, you can also Drag & Drop `start.bat` onto your desktop **while holding the `alt` key**, this will create a shortcut to it

<img width="499" height="163" alt="image_2026-07-25_11-47-53" src="https://github.com/user-attachments/assets/60d1317a-376c-4fd1-a747-8bbb711775b2" />

(Make sure to hold `alt` while you're doing that, if you don't, you will move/copy `start.bat` and it won't work)

</details>

### Download

The first release is planned for **27.07.2026**

## Discord

**Fewer commands - more buttons**

//todo video here

**PamelloV7** uses **ComponentsV2 and Modals** for much better interactive messages, with markdown formatting, containers, sections, modals, and **buttons right inside of a message** instead of them being stacked below it like with the old components

**All messages are constantly updated**. When you run a command like `/player info`, you will see the current time, song, episode, and all the other info updated in real time. This works with all commands and their messages, **so you should never see outdated info**

**All command responses are ephemeral** and visible only to the user who called them, so your messages are both private and don't clutter the chats 

## Discord Usage Guides

<details open>
<summary><h3>Adding songs</h3></summary>

`/add` `{songs}`: **Adds songs to your queue**

`/add-playlist` `{playlists}`: **Adds songs from playlists to your queue**

For songs you can use their ids, associations, urls, or PEQL points like favorite

For playlists you can use their ids, names, and the favorite point too

#### Some Examples Are

`/add` `https://www.youtube.com/watch?v=nnmIzzCLmrU`

`/add` `5`

`/add` `35,14,22`

`/add` `some,associations`

`/add` `alot,together,35,https://www.youtube.com/watch?v=nnmIzzCLmrU,14,favorite,22,playlist(teto)`

</details>

<details>
<summary><h3>Managing a player</h3></summary>

`/player info`: **Brings up real-time interactive player info**

Here you can see your selected player's playback time, current song, episode, queue modes, and a few action buttons

If you don't have a selected player, you can create one with a button inside of the message, or choose an available one the same way

What the buttons do:

- `Pause`: Pauses/Resumes the playback
- `Next Episode`: Skips to the next episode in a song, only appears if the song has episodes
- `Add Songs`: Brings up a modal for you to query more songs, you can paste urls there or any other PEQL queries
- `Rewind`: Brings up a modal in which you can enter the time you want to rewind the currently playing song to
- `Skip`: Just skips the current song

Queue modes:

- `Random`: Play queue songs in a random order
- `Reversed`: Play queue songs in a reversed order
- `No Leftovers`: Remove songs from the queue after they are played
- `Feed Random`: Feed the queue with random songs from the database when it gets empty

Also at the bottom you can see the currently connected players, and who is listening to them

There is also the `/player pause-toggle` command that just toggles the pause state, and responds with an interactive message to switch it back when you want

</details>

<details>
<summary><h3>Managing a queue</h3></summary>
  
`/queue`: **Brings up a real-time interactive queue**

Here you can see your selected player's queue songs, as well as some action buttons

What the buttons do:

- `Add Songs`: Brings up a modal for you to query more songs, you can paste urls there or any other PEQL queries
- `Edit`: Brings up a modal with which you can edit the queue by changing the songs order, removing songs, or adding new ones with PEQL (so ids, urls, points, etc.) into any position
- `Go-To`: Brings up a modal to select a position to jump to right now, and it has an option to get back to the current song after the new one ends
- `Set Next`: Brings up a modal to select a position that will be played after the current song ends

> You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number

</details>

<details>
<summary><h3>Managing songs</h3></summary>

`/song info` `{song?}`: **Brings up real-time interactive song info**

> Accepts a song as a parameter, when not specified uses your `current` song

Here you can see the song name, cover, id, addition date, adder user, some action buttons and dedicated sections

What the buttons do:

- `Rename`: Brings up a modal for you to input the new name
- `Change Cover`: Not implemented yet
- `Reset`: Reset the song basic info like name, cover, and episodes to its source
- `Add to queue`: Adds this song to your current queue, not available if you don't have one

And about the sections:

**Associations**: A list of associations for that song

`Edit`: Brings up a modal for you to change/add/remove associations

**Favorite By Users**: A list of users that added this song to their favorites

`Add`: Add this song to your favorites

**Included In Playlists**: A list of playlists this song is included in

`Remove All`: Remove this song from all playlists

**Sources**: A list of sources for that song

`Select`: Brings up a modal to select the main source for that song, this source will be used when resetting its info

**Episodes**: Displays the count of episodes that song has, hidden if the song doesn't have any

`Show`: Sends the same message as the `/song episode list` command, more about it in [Managing song episodes](#managing-song-episodes)

</details>

<details>
<summary><h3>Managing playlists</h3></summary>

`/playlist info` `{playlist}`: **Brings up real-time interactive playlist info**

> Accepts a playlist as a required parameter

Here you can see the playlist id, name, addition date, owner user, protection state, and all of its songs, as well as some action buttons

What the buttons do:

- `Add to queue`: Adds this playlist songs to your current queue, not available if you don't have one
- `Rename`: Brings up a modal for you to input the new name
- `Edit`: Brings up a modal with which you can edit the playlist by changing the songs order, removing songs, or adding new ones with PEQL (so ids, urls, points, etc.) into any position

Playlists have only one owner, but can be browsed / used by everyone. You can also protect your playlist from changes by other users, or leave it public

</details>

<details>
<summary><h3>Managing song episodes</h3></summary>

`/song episode list` `{song?}`: **Brings up a real-time interactive song episodes list**

> Accepts a song as a parameter, when not specified uses your `current` song

Here you can see all of the song episodes, their start positions, start time, and an episode action button, as well as some general action buttons and mode switch buttons

There are 3 modes `song episode list` can be in, which determine what each episode action button does:
- `Edit`: Edit the episode info, like its name, start time, and its auto skip status
- `Delete`: Delete the episode from the song
- `Rewind`: Rewinds the playback to the episode, only available if this is the current song

The buttons to switch modes are located below the episodes list itself

Other action buttons:
- `Add Episode`: Create a new episode and add it to the song
- `Reset`: Reset the episodes of this song to its selected source

> You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number

</details>

<details>
<summary><h3>Managing favorite songs & playlists</h3></summary>

`/song favorite list` `{user?}`: **Brings up a real-time interactive favorite songs list**

> Accepts a user as a parameter, when not specified uses your `current` user

Here you can see all of your or another user's favorite songs, and a few action buttons

What the buttons do:
- `Edit`: Edit your favorite songs, so move/remove/add songs with PEQL (so ids, urls, points, etc.). Not available if these are not your favorites
- `Clear`: Clear your favorite songs. Not available if these are not your favorites
- `Add all to queue`: Add all of your favorite songs to your current queue, not available if you don't have one

> You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number

</details>

<details>
<summary><h3>Managing your user</h3></summary>

`/user info` `{user?}`: **Brings up real-time interactive user info**

> Accepts a user as a parameter, when not specified uses your `current` user

Here you can see the user join date (the first time the user interacted with Pamello), favorite songs and playlists count, and an authorizations preview

What the "Show" buttons do:
- **For Favorite Songs**: Sends the same message as `/song favorite list` does
- **For Favorite Playlists**: Sends the same message as `/playlist favorite list` does
- **For Authorizations**: Sends the same message as `/user authorization list` does

`/user authorization list` `{user?}`: **Brings up a real-time interactive user authorizations list**

> Accepts a user as a parameter, when not specified uses your `current` user

Here you can see all authorizations of the user, each with its action button, as well as some general action buttons

By adding an authorization to your user, you give it the right to be recognized as your user

By adding multiple Discord authorizations you can bind multiple Discord accounts to one Pamello user, and have all of your data synchronized between them, because they will be recognized as the same user

> Adding an authorization doesn't require any confirmation from its side

There are 2 modes `user authorization list` can be in, which determine what each authorization action button does:
- `Select`: Select an authorization for the user
- `Delete`: Delete the authorization from the user

The buttons to switch modes are located below the authorizations list itself

Other action buttons:
- `Add Authorization`: Create a new authorization and add it to your user
- `Reset`: //todo describe this button, the old text here was copy-pasted from the episodes section

> You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number

</details>

## Playback

**Lag free, fast playback** with caching and chunking

> *Note: "Lag free" means from the server (bot) side, but i can't control the Discord side or any other app side*

Before playing a song for the first time the bot **downloads and saves it locally**. This might take slightly more time (usually still just 2-4 seconds), but when this same song is played again **it will start playing right away**

> *Note: If at any point your audio files collection grows large, you can just delete all of it (or some of it), the bot won't break and will just re-download any requested songs again*

During the playback itself, instead of loading the full song, **Pamello will load it in small chunks**, drastically **optimizing load time and memory usage**

**Players & Speakers** are separated, and the players aren't bound to any Discord guild, **they are global entities**

You can have as many speakers connected in one guild as you are willing to create accounts for. Discord doesn't allow one bot to be in many voice channels of a single guild, so Pamello will use other speaker accounts in that case 

<details>
<summary>More about Players & Speakers use cases</summary>

Some of the examples of what you can do with it:

- Listen to music in one guild, switch to another, and bring back your old player
- Connect many speakers to many voice channels with the same player, and you can have the same music playing and controllable from all of them
- Listen to music on Discord and share the radio url with someone else, now they can hear your music and you can still control it from Discord
- Connect many speakers to many voice channels but with different players, and you can have separate players with their own music in the same or different guilds

</details>

## Database

**Everything is browsable and global**

Pamello stores all of your data in its own database, which makes everything browsable and global, **not bound to any Discord guild or user**

Pamello recognizes your Discord user as a Pamello user, and all of the songs you add, your favorites, and the playlists you own are associated with that Pamello user

You can authorize a single Pamello user with multiple Discord (and osu!) accounts. For example, if you have a bunch of alt Discord accounts, all of them can be added to your single Pamello user, and you will have the same favorites, owned playlists, and everything else on all of them

## PEQL

**Pamello Entity Query Language**, designed to balance between ease of use and functionality

**PEQL is used everywhere** you are asked for any entity, like a **song, playlist, episode, user**, etc.

### For a non technical user that means

In any place where you are asked for a song or another entity, you can write its id/association/url, and separate multiple ones with commas (like writing a few song urls into the `/add` command so you add multiple songs at once, or using urls / associations while editing a queue, etc.)

### For a technical user that means

In any place where you are asked for a song or another entity, you can use ids, value points, and operators. For example:
- `random` - a random entity based on context, like a song in the context of `/add`, or a playlist in the case of `/add-playlist`
- `favorite:2-5` - all songs from position 2 to 5 in your favorites
- `favorite:random*10` - 10 random songs from your favorites

and there is more you can do, you can check out the full list below if you want to

<details>
<summary><h3>All PEQL Operators & Points</h3></summary>

#### Operators

- `:` `{range}` - **Indexation** - Selects entities in the specified `range` from a query
- `*` `{count}` - **Multiplication** - Repeats a query the specified `count` of times
- `#` `{filter}` - **Filter** - Applies some filter to a query, not implemented yet

#### Points

`users`
- `Id`
- `Name`
- `random`: Random user
- `current` | `me`: Your user

`songs`
- `Id`
- `Name`
- `current`: Current song from the selected player queue
- `random`: Random song from the database
- `queue`: All songs from the selected player queue
- `favorite(user?: of)`: All your favorite songs by default, or of a specified user
- `added(user?: by)`: All your added songs by default, or of a specified user
- `playlist(playlist: playlist)`: All songs from a playlist, playlist is required here

`playlists`
- `Id`
- `Name`
- `random`: Random playlist from the database
- `favorite(user?: of)`: All your favorite playlists by default, or of a specified user
- `added(user?: by)`: All your added playlists by default, or of a specified user

`episodes`
- `Id`
- `Name`
- `random`: Random episode from the database
- `current`: Current episode from the current song in the selected player queue
- `song(song: song)`: All episodes from a song, song is required here

`players`
- `Id`
- `Name`
- `random`: Random available player
- `current` | `selected`: Currently selected player

`speakers`
- `Id`
- `Name`
- `current`: All speakers that the user currently listens to

> _Note: Usually you won't need to specify a provider like `songs` and write a full query like `songs$current`, because the provider will be known from context. But when it isn't, then you need to write the full query_

</details>

## Radio

**Play audio through an HTTP MP3 Radio and listen to it anywhere**

You can connect a speaker to the internet, give it a name, and have it available at `https://yourhost/Audio/Out/yourspeakername`. Then you can give this url to any app that supports internet radio playback (like Euro Truck Simulator 2 for example), or open it **in a browser directly**

Even with an internet speaker connected **you can still connect a Discord speaker** too if you want

There are some things you have to understand though:

- You will have to open a port on your router to allow internet connections to this speaker
- The apps that you play the radio to control the buffer size themselves. Because of that you can sometimes have 2-10 seconds of delay in different apps, this is the client side buffer and it's not something i can control from the server side
- Currently it only plays HTTP MP3 320kbps, i will add more configurability to this in the future though

## Updates

**Automatic bot updates**

Just run `update.bat`/`update.sh` when you want to update your bot, updates will install automatically if available

**Automatic dependencies update**

Pamello automatically updates dependencies on each startup, specifically: `yt-dlp`, `ffmpeg`, `ffprobe`, `opus`, `sodium`, and `dave`

---

## Approaches

I started development of PamelloV7 back in 11.2024, and it also has 6 previous iterations starting from 05.2022. So i was working on Pamello for a long time just for myself and my friends, and now i want to try making it for other people too. This isn't some side project, so you can be sure i won't abandon it

I don't use AI to write any significant code, documentation (like this whole README), or to design any architecture. I do all of that myself out of principle

I understand that my goals will take a lot of time, but this is what i want to do. These features are what i want PamelloV7 to have. This is my vision of it

## Goals

### Framework 3.0

A full rewrite of the current framework, which i'm doing right now

**PamelloV7 2.0** already has a lot of features, but there are also a lot of features planned, and many of them are really huge, like the ones i list as the other goals here

All of these features are a lot of work, and implementing them requires a great and reliable framework at the base. The 2.0 version of the framework has a lot of problems that will specifically backfire in the long run, and that's why i'm fully rewriting it, and will rewrite Pamello itself as 3.0 too, revising all of the past mistakes and designing the new framework in a more resilient and future proof way

That is also why Modules & API won't be "released" in 2.0. The framework and the API will change a lot in 3.0, and 3.0 is the version i want to focus on writing the documentation for

The reason why i'm releasing 2.0 before 3.0 is that 3.0 will take a lot of time, and i have been putting the release off for long enough already, i just want to release it. But that's not the whole reason, i also plan on getting feedback from the released 2.0 version, and fixing anything i can in 2.X.X. And not only fixes, any new features that i consider reasonable to add in 2.0, i will add in 2.0

3.0 will change a lot on the code side, and will add some features and fix some bugs just by replacing 2.0, but you as a user don't have to worry about anything breaking, i'm working on migrating all of the data from 2.0 to 3.0 without any changes. Though as a developer who was using the unreleased API & Modules, you do have something to worry about, the framework and the API change completely, and you will have to rewrite everything

### Launcher App

A launcher to manage installations / updates / configuration of the Server, Client, and Modules with a GUI

### Modules

C# Modules that can be written by the community, published, and available for everyone to discover & install in the **Launcher**

### API

An API with C#, TS, JS, and Python wrappers, to allow the community to create integrations with any apps they would want to

### Client App

A fully functional client app, also with the ability to listen to music in it directly, basically like a music player but with a lot of additional features

### Interactive Audio

An interactive audio system, the map of which you can look at in the GUI of the Pamello client (or via API), and edit it in real time, adding effects and routing audio in any way you would want to

### History

History that you can easily browse, recall basically any significant actions, revert some of them, and view a "Spotify like" rewind
