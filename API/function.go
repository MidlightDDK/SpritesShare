package spritesshare

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"regexp"
	"strconv"
	"strings"
	"time"

	firestore "cloud.google.com/go/firestore"
	firebase "firebase.google.com/go"
	"github.com/GoogleCloudPlatform/functions-framework-go/functions"
	"google.golang.org/api/iterator"
)

type normalMessage struct {
	Message string `json:"message"`
}

func init() {
	functions.HTTP("SpritesShareHTTP", spritesShareHTTP)
}

// ThoughtsHTTP is an HTTP Cloud Function with a request parameter.
func spritesShareHTTP(w http.ResponseWriter, r *http.Request) {
	// Initial setup
	w.Header().Set("Content-Type", "application/json")
	pathParams := strings.Split(r.URL.Path, "/")

	// Firestore connection
	ctx := context.Background()
	conf := &firebase.Config{ProjectID: "sprites-share"}
	app, err := firebase.NewApp(ctx, conf)
	if err != nil {
		w.WriteHeader(http.StatusBadRequest)
		data := normalMessage{Message: fmt.Sprintf("%v", err)}
		json.NewEncoder(w).Encode(data)
		return
	}

	client, err := app.Firestore(ctx)
	if err != nil {
		w.WriteHeader(http.StatusBadRequest)
		data := normalMessage{Message: fmt.Sprintf("%v", err)}
		json.NewEncoder(w).Encode(data)
		return
	}
	defer client.Close()

	// Routing
	if pathParams[1] == "" && len(pathParams) == 2 && r.Method == "GET" {
		var data struct {
			Message string    `json:"message"`
			Routes  [4]string `json:"routes"`
		}
		data.Message = "Sprites Share API - API for sprites sharing"
		data.Routes[0] = "GET /sprites?limit=X&asc=Y - ListAllSprites"
		data.Routes[1] = "POST /sprites - AddSprites"
		data.Routes[2] = "GET /sprites/:id - GetSprites"
		data.Routes[3] = "POST /sprites/:id - RateSprites"
		w.WriteHeader(http.StatusOK)
		json.NewEncoder(w).Encode(data)
		return
	} else if pathParams[1] == "sprites" {
		if len(pathParams) == 2 {
			if r.Method == "GET" {
				queryLimit, err := strconv.Atoi(r.URL.Query().Get("limit"))
				if err != nil || queryLimit == 0 {
					queryLimit = 100
				}

				queryAsc := firestore.Desc
				if r.URL.Query().Get("asc") == "true" {
					queryAsc = firestore.Asc
				}

				var dateCreated interface{}
				if r.URL.Query().Get("lastItem") != "" {
					dsnap, err := client.Collection("sprites").Doc(r.URL.Query().Get("lastItem")).Get(ctx)
					if err != nil {
						data := normalMessage{Message: fmt.Sprintf("%v", err)}
						w.WriteHeader(http.StatusBadRequest)
						json.NewEncoder(w).Encode(data)
						return
					} else {
						dateCreated = dsnap.Data()["dateCreated"]
					}
				}

				// Document ref
				var docs []map[string]interface{}
				var queryCollection firestore.Query
				collectionRef := client.Collection("sprites")

				// Get the tag
				hasTag := r.URL.Query().Get("tag") != ""
				if hasTag {
					queryCollection = collectionRef.Where("tags", "array-contains", r.URL.Query().Get("tag"))
				}

				// Order by
				if !hasTag {
					queryCollection = collectionRef.OrderBy("dateCreated", queryAsc)
				} else {
					queryCollection = queryCollection.OrderBy("dateCreated", queryAsc)
				}

				// Pagination
				if dateCreated != nil {
					queryCollection = queryCollection.StartAfter(dateCreated)
				}

				// Total query
				iter := queryCollection.Limit(queryLimit).Documents(ctx)

				for {
					doc, err := iter.Next()
					if err == iterator.Done {
						break
					}
					if err != nil {
						data := normalMessage{Message: fmt.Sprintf("%v", err)}
						w.WriteHeader(http.StatusBadRequest)
						json.NewEncoder(w).Encode(data)
						return
					}

					docs = append(docs, doc.Data())
					docs[len(docs)-1]["id"] = doc.Ref.ID
				}

				w.WriteHeader(http.StatusOK)
				json.NewEncoder(w).Encode(docs)
				return
			} else if r.Method == "POST" {
				// Retrieve BODY params
				var d struct {
					Name        string   `json:"name"`
					Content     string   `json:"content"`
					Author      string   `json:"author"`
					Description string   `json:"description"`
					DimensionX  uint16   `json:"dimensionX"`
					DimensionY  uint16   `json:"dimensionY"`
					Tags        []string `json:"tags"`
				}
				if err := json.NewDecoder(r.Body).Decode(&d); err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.Name == "" {
					data := normalMessage{Message: "'name' string body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.Content == "" {
					data := normalMessage{Message: "'content' string body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.Author == "" {
					data := normalMessage{Message: "'author' string body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.Description == "" {
					data := normalMessage{Message: "'description' string body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.DimensionX == 0 {
					data := normalMessage{Message: "'dimensionX' uint16 body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.DimensionY == 0 {
					data := normalMessage{Message: "'dimensionY' uint16 body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				}

				// Validate tags
				regTester, _ := regexp.Compile("[a-zA-Z0-9]+")

				for i := 0; i < len(d.Tags); i++ {
					if !regTester.MatchString(d.Tags[i]) {
						data := normalMessage{Message: "'tags' must only contain characters in range [a-zA-Z0-9]"}
						w.WriteHeader(http.StatusBadRequest)
						json.NewEncoder(w).Encode(data)
						return
					}
				}

				// Add to Database
				_, _, err := client.Collection("sprites").Add(ctx, map[string]interface{}{
					"dateCreated": time.Now().UTC(),
					"name":        d.Name,
					"content":     d.Content,
					"author":      d.Author,
					"description": d.Description,
					"dimensionX":  d.DimensionX,
					"dimensionY":  d.DimensionY,
					"tags":        d.Tags,
				})
				if err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else {
					data := normalMessage{Message: "Sprites created!"}
					w.WriteHeader(http.StatusOK)
					json.NewEncoder(w).Encode(data)
					return
				}
			}
		} else if len(pathParams) == 3 {
			if r.Method == "GET" {
				dsnap, err := client.Collection("sprites").Doc(pathParams[2]).Get(ctx)
				if err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else {
					data := dsnap.Data()
					data["id"] = dsnap.Ref.ID
					w.WriteHeader(http.StatusOK)
					json.NewEncoder(w).Encode(data)
					return
				}
			} else if r.Method == "POST" {
				// Retrieve BODY params
				var d struct {
					Rating uint8 `json:"rating"`
				}

				if err := json.NewDecoder(r.Body).Decode(&d); err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.Rating == 0 {
					data := normalMessage{Message: "'rating' uint8 body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				}

				// Get IP address
				ipAddress := strings.Split(r.RemoteAddr, ":")
				extractedIp := strings.Replace(ipAddress[0], ".", " ", -1)

				// Add to Database
				_, err := client.Collection("sprites").Doc(pathParams[2]).Update(ctx, []firestore.Update{
					{
						Path:  "dateCreated",
						Value: time.Now().UTC(),
					},
					{
						Path:  "ratings." + extractedIp,
						Value: d.Rating,
					},
				})
				if err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else {
					data := normalMessage{Message: "Sprites rated!"}
					w.WriteHeader(http.StatusOK)
					json.NewEncoder(w).Encode(data)
					return
				}
			}
		}
	}

	// Default response
	w.WriteHeader(http.StatusBadRequest)
	data := normalMessage{Message: "Unknown route"}
	json.NewEncoder(w).Encode(data)
}
